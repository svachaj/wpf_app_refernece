using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using Microsoft.SqlServer.Server;

namespace dotMath
{
    /// <summary>
    /// EqCompiler is the class that takes the parsed tokens and turns them
    /// into a network of pre-compiled objects that perform the designated
    /// functions.
    /// </summary>
    /// <remarks>
    /// Copyright (c) 2001-2004, Stephen Hebert
    /// All rights reserved.
    /// 
    /// 
    /// Redistribution and use in source and binary forms, with or without modification, 
    /// are permitted provided that the following conditions are met:
    /// 
    /// Redistributions of source code must retain the above copyright notice, 
    /// this list of conditions and the following disclaimer. 
    /// 
    /// Redistributions in binary form must reproduce the above 
    /// copyright notice, this list of conditions and the following disclaimer 
    /// in the documentation and/or other materials provided with the distribution. 
    /// 
    /// Neither the name of the .Math, nor the names of its contributors 
    /// may be used to endorse or promote products derived from this software without 
    /// specific prior written permission. 
    /// 
    /// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
    /// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED 
    /// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
    /// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS 
    /// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
    /// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
    /// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; 
    /// OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
    /// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
    /// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
    /// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
    /// </remarks>
    public class EqCompiler
    {
        private string m_sEquation;
        private CValue m_Function;
        private Parser.Token m_currentToken;
        private Parser.Token m_nextToken;
        private IEnumerator m_enumTokens;
        private Dictionary<string, CVariable> m_slVariables = new Dictionary<string, CVariable>();
        private static Dictionary<string, CFunction> m_slFunctions = new Dictionary<string, CFunction>();
        //private SortedList m_slOperations = new SortedList();
        private bool _UseRegionalSettings = false;
        private string m_sCommaSep;


        private COperator[] m_aOps;

        static EqCompiler()
        {
            CFunctionLibrary.AddFunctions();
        }


        #region Operations and Compiling Functions

        /// <summary>
        /// CSignNeg provides negative number functionality
        /// within an equation. 
        /// </summary>
        private class CSignNeg : CValue
        {
            CValue m_oValue;

            /// <summary>
            /// CSignNeg constructor:  Grabs onto the assigned
            ///		CValue object and retains it for processing
            ///		requested operations.
            /// </summary>
            /// <param name="oValue">Child operation this object operates upon.</param>
            public CSignNeg(CValue oValue)
            {
                m_oValue = oValue;
            }

            /// <summary>
            /// GetValue():  Performs the negative operation on the child operation and returns the value.
            /// </summary>
            /// <returns>A double value evaluated and returned with the opposite sign.</returns>
            public override double GetValue()
            {
                return m_oValue.GetValue() * -1;
            }

            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_oValue.SetExecutionContext(context);
            }
        }

        /// <summary>
        /// Paren() : This method evaluates Parenthesis in the equation and
        ///		insures they are handled properly according to the Order of Operations. Because this is last in the chain,
        ///		it also evaluates Variable and Function names.
        /// </summary>
        /// <returns>CValue object that holds an operation.</returns>
        private CValue Paren()
        {
            //bool bFunc = false;
            CValue oValue = null;

            if (m_currentToken.ToString() == "(")
            {
                PositionNextToken();

                oValue = Relational();

                if (m_currentToken.ToString() == m_sCommaSep)
                    return oValue;

                if (m_currentToken.ToString() != ")")
                    throw new ApplicationException("Unmatched parenthesis in equation.");

                //PositionNextToken();
            }
            else
            {
                switch (m_currentToken.TokenType)
                {
                    case Parser.CharType.CT_NUMBER:
                        oValue = new CNumber(m_currentToken.ToString());
                        break;

                    case Parser.CharType.CT_STRING:
                        oValue = new CString(m_currentToken.ToString());
                        break;

                    case Parser.CharType.CT_LETTER:
                        {
                            if (m_nextToken.ToString() == "(")
                            {
                                CFunction oFunc = null;
                                if (!m_slFunctions.TryGetValue(m_currentToken.ToString().ToLower(), out oFunc))
                                    throw new ApplicationException("Function not found - " + m_currentToken.ToString());

                                List<CValue> alValues = new List<CValue>();

                                PositionNextToken();
                                bool first = true;
                                do
                                {
                                    if (first && m_nextToken.ToString() == ")")
                                    {
                                        PositionNextToken();
                                        oValue = null;
                                    }
                                    else
                                    {
                                        PositionNextToken();
                                        oValue = Relational(); //SDH: Paren();
                                    }
                                    if (oValue != null)
                                        alValues.Add(oValue);
                                    first = false;
                                } while (m_currentToken.ToString() == m_sCommaSep);

                                //bFunc = true;


                                oValue = oFunc.CreateInstance(alValues);
                            }
                            else
                                oValue = GetVariableByName(m_currentToken.ToString());

                            break;
                        }
                }

            }

            //if (!bFunc)
            PositionNextToken();

            return oValue;

        }


        /// <summary>
        /// Sign():  This method detects the existence of sign operators before
        ///		a number or variable.  
        /// </summary>
        /// <returns>CValue object representing an operation.</returns>
        private CValue Sign()
        {
            bool bNeg = false;
            Parser.Token oToken = null;
            if (m_currentToken == "+" || m_currentToken == "-")
            {
                oToken = m_currentToken;
                bNeg = (m_currentToken == "-");
                PositionNextToken();
            }
            //CValue oFunc = Function();
            // sdh: should be function when ready.
            CValue oFunc = Paren();


            if (bNeg)
            {
                CheckParms(oToken, oFunc);
                oFunc = new CSignNeg(oFunc);
            }

            return oFunc;


        }

        /// <summary>
        /// Power():  Detects the operation to raise one number to the power
        ///		of another (a^2).
        /// </summary>
        /// <returns>CValue object representing an operation.</returns>
        private CValue Power()
        {
            CValue oValue = Sign();

            while (m_currentToken == "^")
            {
                Parser.Token oOp = m_currentToken;

                PositionNextToken();

                CValue oNextVal = Sign();

                CheckParms(oOp, oValue, oNextVal);
                oValue = OpFactory(oOp, oValue, oNextVal);
            }

            return oValue;
        }

        /// <summary>
        /// MultDiv(): Detects the operation to perform multiplication or division.
        /// </summary>
        /// <returns>CValue object representing an operation.</returns>
        private CValue MultDiv()
        {
            CValue oValue = Power();

            while (m_currentToken == "*" || m_currentToken == "/")
            {
                Parser.Token oOp = m_currentToken;

                PositionNextToken();

                CValue oNextVal = Power();

                CheckParms(oOp, oValue, oNextVal);
                oValue = OpFactory(oOp, oValue, oNextVal);
            }

            return oValue;
        }

        /// <summary>
        /// AddSub(): Detects the operation to perform addition or substraction.
        /// </summary>
        /// <returns>CValue object representing an operation.</returns>
        private CValue AddSub()
        {
            CValue oValue = MultDiv();

            while (m_currentToken == "+" || m_currentToken == "-")
            {
                Parser.Token oOp = m_currentToken;
                PositionNextToken();

                CValue oNextVal = MultDiv();

                CheckParms(oOp, oValue, oNextVal);
                oValue = OpFactory(oOp, oValue, oNextVal);
            }

            return oValue;
        }

        /// <summary>
        /// Relational():  Detects the operation to perform a relational operator (=, etc.).
        /// </summary>
        /// <returns>CValue object representing an operation.</returns>
        private CValue Relational()
        {
            CValue oValue = AddSub();

            while (m_currentToken == "&&" ||
                m_currentToken == "||" ||
                m_currentToken == "=" ||
                m_currentToken == "==" ||
                m_currentToken == "<" ||
                m_currentToken == ">" ||
                m_currentToken == "<=" ||
                m_currentToken == ">=" ||
                m_currentToken == "!=" ||
                m_currentToken == "<>")
            {
                Parser.Token oOp = m_currentToken;
                PositionNextToken();
                CValue oNextVal = Relational();

                CheckParms(oOp, oValue, oNextVal);
                oValue = OpFactory(oOp, oValue, oNextVal);

            }

            return oValue;
        }

        /// <summary>
        /// OpFactor(...): Reads the passed operator, identifies the associated implementation object
        ///		and requests an operation object to be used in evaluating the equation.
        /// </summary>
        /// <param name="oSourceOp">Parser.Token object representing the operator in question.</param>
        /// <param name="oValue1">The first value object to be operated on.</param>
        /// <param name="oValue2">The second value object to be operated on.</param>
        /// <returns>CValue object representing an operation.</returns>
        private CValue OpFactory(Parser.Token oSourceOp, CValue oValue1, CValue oValue2)
        {

            foreach (COperator oOp in m_aOps)
            {

                if (oOp.IsMatch(oSourceOp))
                    return oOp.Factory(oValue1, oValue2);
            }

            throw new ApplicationException("Invalid operator in equation.");

        }


        #endregion

        #region Operators

        /// <summary>
        /// CAdd class: Implements the Add(+) operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class CAdd : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            /// <summary>
            /// Initializes a new instance of the <see cref="CAdd"/> class.
            /// </summary>
            public CAdd()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CAdd"/> class.
            /// </summary>
            /// <param name="arg1">The arg1.</param>
            /// <param name="arg2">The arg2.</param>
            public CAdd(CValue arg1, CValue arg2)
            {

                CheckParms("+", arg1, arg2);

                m_arg1 = arg1;
                m_arg2 = arg2;
            }
            public override double GetValue()
            {
                return m_arg1.GetValue() + m_arg2.GetValue();
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == "+");
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CAdd(arg1, arg2);
            }

            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }

        /// <summary>
        /// CSubtract class: Implements the Subtract(-) operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class CSubtract : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public CSubtract()
            {
            }
            public CSubtract(CValue arg1, CValue arg2)
            {
                CheckParms("-", arg1, arg2);
                m_arg1 = arg1;
                m_arg2 = arg2;
            }
            public override double GetValue()
            {
                return m_arg1.GetValue() - m_arg2.GetValue();
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return tkn.ToString() == "-";
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CSubtract(arg1, arg2);
            }

            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }


        /// <summary>
        /// CLessThan class: Implements the LessThan operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>

        private class CLessThan : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public CLessThan()
            {
            }

            public CLessThan(CValue arg1, CValue arg2)
            {
                CheckParms("<", arg1, arg2);

                m_arg1 = arg1;
                m_arg2 = arg2;
            }

            public override double GetValue()
            {
                if (m_arg1.GetValue() < m_arg2.GetValue())
                    return 1;
                else
                    return 0;
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == "<");
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CLessThan(arg1, arg2);
            }
            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }


        /// <summary>
        /// COr class: Implements the boolean Or(||) operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class COr : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public COr()
            {
            }

            public COr(CValue arg1, CValue arg2)
            {
                CheckParms("||", arg1, arg2);

                m_arg1 = arg1;
                m_arg2 = arg2;
            }

            public override double GetValue()
            {
                if (m_arg1.GetValue() != 0 || m_arg2.GetValue() != 0)
                    return 1;
                else
                    return 0;
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == "||");
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new COr(arg1, arg2);
            }
            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }


        /// <summary>
        /// CAnd class: Implements the boolean And(C# syntax) operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class CAnd : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public CAnd()
            {
            }

            public CAnd(CValue arg1, CValue arg2)
            {
                CheckParms("&&", arg1, arg2);

                m_arg1 = arg1;
                m_arg2 = arg2;
            }

            public override double GetValue()
            {
                if (m_arg1.GetValue() != 0 && m_arg2.GetValue() != 0)
                    return 1;
                else
                    return 0;
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == "&&");
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CAnd(arg1, arg2);
            }
            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }


        /// <summary>
        /// CEqual class: Implements the binary Equal(==) operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class CEqual : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public CEqual()
            {
            }

            public CEqual(CValue arg1, CValue arg2)
            {
                CheckParms("= or ==", arg1, arg2);
                m_arg1 = arg1;
                m_arg2 = arg2;
            }

            public override double GetValue()
            {
                if (m_arg1.GetValue() == m_arg2.GetValue())
                    return 1;
                else
                    return 0;
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == "=" || tkn.ToString() == "==");
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CEqual(arg1, arg2);
            }
            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }


        /// <summary>
        /// CNotEqual class: Implements the NotEqual operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class CNotEqual : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public CNotEqual()
            {
            }

            public CNotEqual(CValue arg1, CValue arg2)
            {
                CheckParms("<> or !=", arg1, arg2);
                m_arg1 = arg1;
                m_arg2 = arg2;
            }

            public override double GetValue()
            {
                if (m_arg1.GetValue() != m_arg2.GetValue())
                    return 1;
                else
                    return 0;
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == "!=" || tkn.ToString() == "<>");
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CNotEqual(arg1, arg2);
            }
            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }


        /// <summary>
        /// CGreaterThan class: Implements the Greater Than(>) operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class CGreaterThan : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public CGreaterThan()
            {
            }

            public CGreaterThan(CValue arg1, CValue arg2)
            {
                CheckParms(">", arg1, arg2);

                m_arg1 = arg1;
                m_arg2 = arg2;
            }

            public override double GetValue()
            {
                if (m_arg1.GetValue() > m_arg2.GetValue())
                    return 1;
                else
                    return 0;
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == ">");
            }


            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CGreaterThan(arg1, arg2);
            }
            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }


        /// <summary>
        /// CGreaterThanEq class: Implements the Greater Than or Equal To(>=) operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class CGreaterThanEq : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public CGreaterThanEq()
            {
            }

            public CGreaterThanEq(CValue arg1, CValue arg2)
            {
                CheckParms(">=", arg1, arg2);
                m_arg1 = arg1;
                m_arg2 = arg2;
            }

            public override double GetValue()
            {
                if (m_arg1.GetValue() >= m_arg2.GetValue())
                    return 1;
                else
                    return 0;
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == ">=");
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CGreaterThanEq(arg1, arg2);
            }
            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }


        /// <summary>
        /// CLessThanEq class: Implements the Less Than or Equal To operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class CLessThanEq : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public CLessThanEq()
            {
            }

            public CLessThanEq(CValue arg1, CValue arg2)
            {
                CheckParms("<=", arg1, arg2);

                m_arg1 = arg1;
                m_arg2 = arg2;
            }

            public override double GetValue()
            {
                if (m_arg1.GetValue() <= m_arg2.GetValue())
                    return 1;
                else
                    return 0;
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == "<=");
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CLessThanEq(arg1, arg2);
            }
            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }


        /// <summary>
        /// CMultiply class: Implements the Multiplication(*) operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class CMultiply : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public CMultiply()
            {
            }

            public CMultiply(CValue arg1, CValue arg2)
            {
                CheckParms("*", arg1, arg2);

                m_arg1 = arg1;
                m_arg2 = arg2;
            }
            public override double GetValue()
            {
                return m_arg1.GetValue() * m_arg2.GetValue();
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == "*");
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CMultiply(arg1, arg2);
            }

            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }

        /// <summary>
        /// CDivide class: Implements the Division(/) operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class CDivide : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public CDivide()
            {
            }

            public CDivide(CValue arg1, CValue arg2)
            {
                CheckParms("/", arg1, arg2);

                m_arg1 = arg1;
                m_arg2 = arg2;
            }
            public override double GetValue()
            {
                return m_arg1.GetValue() / m_arg2.GetValue();
            }

            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == "/");
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CDivide(arg1, arg2);
            }

            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }


        /// <summary>
        /// CPower class: Implements the Power(^) operation. Refer to COperator base class
        ///		for a description of the methods.
        /// </summary>
        private class CPower : COperator
        {
            private CValue m_arg1 = null;
            private CValue m_arg2 = null;

            public CPower()
            {
            }

            public CPower(CValue arg1, CValue arg2)
            {
                CheckParms("^", arg1, arg2);

                m_arg1 = arg1;
                m_arg2 = arg2;
            }

            public override double GetValue()
            {
                return Math.Pow(m_arg1.GetValue(), m_arg2.GetValue());
            }


            public override bool IsMatch(Parser.Token tkn)
            {
                return (tkn.ToString() == "^");
            }

            public override COperator Factory(CValue arg1, CValue arg2)
            {
                return new CPower(arg1, arg2);
            }
            /// <summary>
            /// Sets the execution context.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void SetExecutionContext(object context)
            {
                this.Context = context;
                m_arg1.SetExecutionContext(context);
                m_arg2.SetExecutionContext(context);
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// CheckParms( Parser.Token, CValue, CValue) - This method makes certain the arguments are non-null
        /// </summary>
        /// <param name="oToken">Currently processed Parser.Token object</param>
        /// <param name="arg1">CValue argument 1</param>
        /// <param name="arg2">CValue argument 2</param>
        private void CheckParms(Parser.Token oToken, CValue arg1, CValue arg2)
        {
            if (arg1 == null || arg2 == null)
                throw new ApplicationException("Argument not supplied near " + oToken.ToString() + " operation.");
        }

        /// <summary>
        /// CheckParms( Parser.Token, CValue) - This method makes certain the single argument is non-null.
        ///		Raises an exception if it is.
        /// </summary>
        /// <param name="oToken">Parser.Token object</param>
        /// <param name="arg1">CValue argument</param>
        private void CheckParms(Parser.Token oToken, CValue arg1)
        {
            if (arg1 == null)
                throw new ApplicationException("Argument not supplied near " + oToken.ToString() + " operation.");
        }

        /// <summary>
        /// InitFunctions(): Creates all operation functions recognized by the compiler.
        /// </summary>
        private void InitFunctions()
        {
            m_aOps = new COperator[13];

            m_aOps[0] = new CAdd();
            m_aOps[1] = new CSubtract();
            m_aOps[2] = new CMultiply();
            m_aOps[3] = new CDivide();
            m_aOps[4] = new CGreaterThan();
            m_aOps[5] = new CGreaterThanEq();
            m_aOps[6] = new CLessThan();
            m_aOps[7] = new CLessThanEq();
            m_aOps[8] = new CEqual();
            m_aOps[9] = new CNotEqual();
            m_aOps[10] = new CPower();
            m_aOps[11] = new CAnd();
            m_aOps[12] = new COr();
        }

        /// <summary>
        /// PositionNextToken():  Manipulates the current Token position forward in the chain of tokens
        ///		discovered by the parser.
        /// </summary>
        private void PositionNextToken()
        {
            if (m_currentToken == null)
            {
                if (!m_enumTokens.MoveNext())
                    throw new ApplicationException("Invalid equation.");

                m_nextToken = (Parser.Token)m_enumTokens.Current;
            }

            m_currentToken = m_nextToken;

            if (!m_enumTokens.MoveNext())
                m_nextToken = new Parser.Token();
            else
                m_nextToken = (Parser.Token)m_enumTokens.Current;
        }


        #endregion



        /// <summary>
        /// GetVariableByName(string) : This method returns the variable associated with the
        ///		provided name string.
        /// </summary>
        /// <param name="sVarName">string variable name</param>
        /// <returns>CVariable object mapped to the passed variable name</returns>
        private CVariable GetVariableByName(string sVarName)
        {
            if (m_slVariables == null)
                m_slVariables = new Dictionary<string, CVariable>();

            CVariable oVar = null;
            if (!m_slVariables.TryGetValue(sVarName, out oVar))
            {
                oVar = new CVariable(sVarName);
                m_slVariables.Add(sVarName, oVar);
            }

            return oVar;
        }

        /// <summary>
        /// VariableCount property: This property reports the current
        ///		variable count.  It is valid after a 'Compile()' function is executed.
        /// </summary>
        public int VariableCount { get { return m_slVariables.Count; } }

        /// <summary>
        /// SetVariable( string, double):  Sets the object mapped to the string variable name
        ///		to the double value passed.
        /// </summary>
        /// <param name="sVarName">Variable Name</param>
        /// <param name="dValue">New Value for variable</param>
        public void SetVariable(string sVarName, double dValue)
        {
            CVariable oVar = GetVariableByName(sVarName);
            oVar.SetValue(dValue);
        }


        /// <summary>
        /// GetVariableList(): returns a string array containing all the variables that
        ///		have been found by the compiler.
        /// </summary>
        /// <returns>string array of current variable names</returns>
        public List<string> GetVariableList()
        {
            //if (m_slVariables.Count == 0)
            //    return null;

            List<string> ret = new List<string>();
            foreach (var item in m_slVariables.Keys)
            {
                ret.Add(item);
            }
            return ret;
        }


        /// <summary>
        /// EqCompiler() constructor: creates the compiler object with an empty function that returns '0' if evaluated.
        /// </summary>
        /// <param name="bIncludeStandardFunctions">if set to <c>true</c> [b include standard functions].</param>
        public EqCompiler(bool bIncludeStandardFunctions)
        {
            Init("0", bIncludeStandardFunctions, false);
        }

        /// <summary>
        /// EqCompiler(string) constructor: creates the compiler object and sets the current function to the string passed
        /// </summary>
        /// <param name="sEquation">The s equation.</param>
        /// <param name="bIncludeStandardFunctions">if set to <c>true</c> [b include standard functions].</param>
        public EqCompiler(string sEquation, bool bIncludeStandardFunctions)
        {
            Init(sEquation, bIncludeStandardFunctions, false);
        }

        /// <summary>
        /// Eqcompiler(string,bool,bool) constructor: creates the compiler object while setting the current function to the string
        /// being passed. It also declares whether to include standard functions and whether to use regional settings to determine
        /// the decimal separator.
        /// </summary>
        /// <param name="sEquation"></param>
        /// <param name="bIncludeStandardFunctions"></param>
        /// <param name="bUseRegionalSettings"></param>
        public EqCompiler(string sEquation, bool bIncludeStandardFunctions, bool bUseRegionalSettings)
        {
            Init(sEquation, bIncludeStandardFunctions, bUseRegionalSettings);
        }

        private void Init(string sEquation, bool bIncludeStandardFunctions, bool bUseRegionalSettings)
        {
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            SetFunction(sEquation);
            _UseRegionalSettings = bUseRegionalSettings;

            //if( bIncludeStandardFunctions )
            //    CFunctionLibrary.AddFunctions(this);
        }

        /// <summary>
        /// Eqcompiler(bool,bool) constructor: creates the compiler object while declaring whether to include standard functions 
        /// and whether to use regional settings to determine the decimal separator.
        /// </summary>
        /// <param name="bIncludeStandardFunctions"></param>
        /// <param name="bUseRegionalSettings"></param>
        public EqCompiler(bool bIncludeStandardFunctions, bool bUseRegionalSettings)
        {
            Init("0", bIncludeStandardFunctions, bUseRegionalSettings);
        }




        /// <summary>
        /// SetFunction(string): Sets the current function to a passed string.
        /// </summary>
        /// <param name="sEquation">string representing the function being used</param>
        public void SetFunction(string sEquation)
        {
            m_currentToken = null;
            m_nextToken = null;

            m_sEquation = string.Format("({0})", sEquation);
            m_Function = null;
            InitFunctions();

        }


        /// <summary>
        /// Compile():  This function kicks off the process to tokenize the function
        ///		and compile the resulting token set into a runnable form.
        /// </summary>
        public void Compile()
        {
            Parser oParser = new Parser(m_sEquation, _UseRegionalSettings);
            m_enumTokens = oParser.GetTokenEnumerator();
            m_sCommaSep = oParser.CommaSeparator;


            PositionNextToken();

            m_Function = Relational();
        }



        /// <summary>
        /// Calculate():  Calls into the runnable function set to evaluate the function and returns the result.
        /// </summary>
        /// <returns>double value evaluation of the function in its current state</returns>
        public double Calculate()
        {
            if (m_Function == null)
                Compile();

            return m_Function.GetValue();
        }

        public void CheckExpression()
        {
            string expres = m_sEquation;
            int lpc = 0;
            int rpc = 0;
            string parstr = "";

            foreach (char chr in expres)
            {
                if (chr == '(')
                {
                    lpc++;
                    parstr += chr;
                }
                else if (chr == ')')
                { 
                    rpc++;
                    parstr += chr;
                }
            }

            if (lpc != rpc)
            {
                string sMsg = string.Format("Bad parentheses count in expression {0}" + 
                    Environment.NewLine + "Left p. count: {1} , Right p. count: {2}", m_sEquation.ToString(), lpc.ToString(), rpc.ToString());
                throw new ApplicationException(sMsg);
            }

            if(!string.IsNullOrEmpty(parstr) && parstr[0] == ')')
            {
                string sMsg = string.Format("Bad first parentheses in expression {0}", m_sEquation.ToString());
                throw new ApplicationException(sMsg);
            }

            if (!string.IsNullOrEmpty(parstr) && parstr[parstr.Length-1] == '(')
            {
                string sMsg = string.Format("Bad last parentheses in expression {0}", m_sEquation.ToString());
                throw new ApplicationException(sMsg);
            }



        }

        /// <summary>
        /// Calculates the specified argumenty.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="argumenty">The argumenty.</param>
        /// <param name="checkVariables">if set to <c>true</c> [check variables].</param>
        /// <returns></returns>
        public double Calculate(object context, Dictionary<string, double> argumenty, bool checkVariables)
        {
            double ret = 0;
            _lock.EnterWriteLock();
            try
            {
                var vList = GetVariableList();

                Dictionary<string, double> pomArgs = new Dictionary<string, double>();
                Dictionary<string, double> pomArgs2 = new Dictionary<string, double>();
                List<string> pomList = new List<string>();

                CheckExpression();


                //////foreach (KeyValuePair<string, double> av in argumenty) 
                //////{
                //////    pomArgs.Add(av.Key.ToLower(), av.Value);
                //////    pomArgs2.Add(av.Key.Trim(), av.Value);
                //////}


                //////foreach (var item in argumenty)
                //////{
                //////    if (vList.Contains(item.Key))
                //////    {
                //////        SqlContext.Pipe.Send("nastavuji promìnnou ve fázi1: " + item.Key );
                //////        SetVariable(item.Key, item.Value);
                //////        vList.Remove(item.Key);
                //////    }                   
                //////}


                //ggggggggggggggg
                foreach (var item in argumenty)
                {
                    for (int i = 0; i < vList.Count; i++)
                    {
                        if (vList[i] == item.Key)
                        {
                            //SqlContext.Pipe.Send("nastavuji promìnnou ve fázi1: " + item.Key);
                            SetVariable(item.Key, item.Value);
                            vList.Remove(item.Key);
                            i--;
                        }
                        else if (vList[i].ToLower() == item.Key.ToLower())
                        {
                            //SqlContext.Pipe.Send("nastavuji promìnnou ve fázi2: " + item.Key);
                            SetVariable(vList[i], item.Value);
                            vList.Remove(vList[i]);
                            i--;
                        }
                        else if (vList[i] == item.Key.Trim())
                        {
                            //SqlContext.Pipe.Send("nastavuji promìnnou ve fázi3: " + item.Key);
                            SetVariable(item.Key.Trim(), item.Value);
                            vList.Remove(item.Key.Trim());
                            i--;
                        }
                        else if (vList[i].ToLower() == item.Key.ToLower().Trim())
                        {
                            //SqlContext.Pipe.Send("nastavuji promìnnou ve fázi3: " + item.Key);
                            SetVariable(vList[i], item.Value);
                            vList.Remove(vList[i]);
                            i--;
                        }
                    }
                }
                //ggggggggggggggg




                //////foreach (var item in pomArgs2)
                //////{
                //////    if (vList.Contains(item.Key))
                //////    {                        
                //////        vList.Remove(item.Key);
                //////        pomArgs.Remove(item.Key);
                //////    }
                //////}


                //////for (int i = 0; i < vList.Count; i++) 
                //////{
                //////    if (pomArgs.ContainsKey(vList[i].ToLower())) 
                //////    {
                //////        SqlContext.Pipe.Send("nastavuji promìnnou ve fázi2: " + vList[i]);
                //////        SetVariable(vList[i], pomArgs[vList[i].ToLower()]);
                //////        argumenty.Add(vList[i], pomArgs[vList[i].ToLower()]);
                //////        pomList.Add(vList[i]);
                //////    }
                //////}

                //////foreach (string pl in pomList)
                //////{
                //////    vList.Remove(pl);
                //////}


                if (checkVariables && vList.Count > 0)
                {
                    string str = string.Empty;
                    foreach (var item in vList)
                    {
                        str += ";" + item;
                    }
                    string sMsg = string.Format("Unspecified variable {0} in expression {1}", str, m_sEquation.ToString());
                    throw new ApplicationException(sMsg);
                }

                if (m_Function == null)
                    Compile();

                m_Function.SetExecutionContext(context);
                ret = Calculate();
                m_Function.SetExecutionContext(null);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return ret;
        }


        private ReaderWriterLockSlim _lock;

        /// <summary>
        /// AddFunction(CFunction): This member accepts a function object and
        ///		adds it to the compilers set of functions.
        /// </summary>
        /// <param name="oFunc">CFunction object that implements a functionality extension for the compiler.</param>
        public static void AddFunction(CFunction oFunc)
        {
            m_slFunctions.Add(oFunc.GetFunction(), oFunc);
        }
    }


    #region Core Base Classes

    /// <summary>
    /// CBalue class:  This base is the basic building block of
    ///		all operations, variables, functions, etc..  Any object
    ///		may call to a CValue object asking for it to resolve itself
    ///		and return it's processed value.
    /// </summary>
    public abstract class CValue
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public abstract double GetValue();
        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <returns></returns>
        public virtual int GetValueType()
        {
            return 0;
        }
        /// <summary>
        /// GetValueString(): returns the static value when called upon.
        /// </summary>
        /// <returns>string</returns>
        public virtual string GetValueString()
        {
            return GetValue().ToString();
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public object Context
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the execution context.
        /// </summary>
        /// <param name="context">The context.</param>
        public abstract void SetExecutionContext(object context);
    }


    /// <summary>
    /// CVariable class : Derived from CValue, CVariable implements
    ///		a named expression variable, holding the name and associated value.
    ///		This object is accessed when an expression is evaluated or when
    ///		a user sets a variable value.
    /// </summary>
    public class CVariable : CValue
    {
        private string m_sVarName;
        private double m_dValue;

        /// <summary>
        /// CVariable(string) constructor: Creates the object and holds onto the 
        ///		compile-time assigned variable name.
        /// </summary>
        /// <param name="sVarName">Name of the variable within the expression.</param>
        public CVariable(string sVarName)
        {
            m_sVarName = sVarName;
        }

        /// <summary>
        /// CVariable(string,double) constructor: Creates the objects and holds onto the
        ///		compile-time assigned variable name and value.
        /// </summary>
        /// <param name="sVarName">string containing the variable name</param>
        /// <param name="dValue">double containing the assigned variable value</param>
        public CVariable(string sVarName, double dValue)
        {
            m_sVarName = sVarName;
        }

        /// <summary>
        /// SetValue(): Allows the setting of the variables value at runtime.
        /// </summary>
        /// <param name="dValue"></param>
        public void SetValue(double dValue)
        {
            m_dValue = dValue;
        }

        /// <summary>
        /// GetValue(): Returns the value of the variable to the calling client.
        /// </summary>
        /// <returns>double</returns>
        public override double GetValue()
        {
            return m_dValue;
        }

        /// <summary>
        /// Sets the execution context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void SetExecutionContext(object context)
        {
            this.Context = context;
        }
    }


    /// <summary>
    /// CNumber Class:  A CValue-derived class that implements a static
    ///    numeric value in an expression.
    /// </summary>
    public class CNumber : CValue
    {
        private double m_dValue;

        /// <summary>
        /// CNumber(string) constructor:  take a string representation of a static number
        ///		and stores it for future retrieval.
        /// </summary>
        /// <param name="sValue">string/text representation of a number</param>
        public CNumber(string sValue)
        {
            if (sValue.Length > 0)
            {
                if (!Double.TryParse(sValue, out m_dValue))
                {
                    if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ".")
                        sValue = sValue.Replace(",", ".");
                    else if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                        sValue = sValue.Replace(".", ",");
                    Double.TryParse(sValue, out m_dValue);
                }
            }
            else
                m_dValue = 0f;
            //OnTry:
            //        bool tryIt = true;
            //    try
            //    {
            //        if (sValue.Length > 0)
            //            Double.TryParse(sValue, out m_dValue);
            //        else
            //            m_dValue = 0;
            //    }
            //    catch
            //    {
            //        if (tryIt)
            //        {
            //            tryIt = false;
            //            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ".")
            //                sValue = sValue.Replace(",", ".");
            //            else if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
            //                sValue = sValue.Replace(".", ",");

            //            goto OnTry;
            //        }
            //    }
        }

        /// <summary>
        /// CNumber(double) constructor: takes a double represenation of a static number
        /// and stores it for future retrieval.
        /// </summary>
        /// <param name="dValue"></param>
        public CNumber(double dValue)
        {
            m_dValue = dValue;
        }

        /// <summary>
        /// GetValue(): returns the static value when called upon.
        /// </summary>
        /// <returns>double</returns>
        public override double GetValue()
        {
            return m_dValue;
        }

        /// <summary>
        /// Sets the execution context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void SetExecutionContext(object context)
        {
            this.Context = context;
        }
    }

    /// <summary>
    /// CNumber Class:  A CValue-derived class that implements a static
    ///    numeric value in an expression.
    /// </summary>
    public class CString : CValue
    {
        private string m_dValue;

        /// <summary>
        /// CNumber(string) constructor:  take a string representation of a static number
        ///		and stores it for future retrieval.
        /// </summary>
        /// <param name="sValue">string/text representation of a number</param>
        public CString(string sValue)
        {
            if (sValue != null && sValue.Length > 2)
                m_dValue = sValue.Substring(1, sValue.Length - 2);
            else
                m_dValue = string.Empty;
        }

        /// <summary>
        /// GetValue(): returns the static value when called upon.
        /// </summary>
        /// <returns>double</returns>
        public override double GetValue()
        {
            return 0;
        }

        /// <summary>
        /// GetValue(): returns the static value when called upon.
        /// </summary>
        /// <returns>double</returns>
        public override string GetValueString()
        {
            return m_dValue;
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <returns></returns>
        public override int GetValueType()
        {
            return 1;
        }

        /// <summary>
        /// Sets the execution context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void SetExecutionContext(object context)
        {
            this.Context = context;
        }
    }

    /// <summary>
    /// COperator class: A CValue derived class responsible for identifying
    ///		and implementing operations during the parsing and evaluation processes.
    /// </summary>
    public abstract class COperator : CValue
    {
        /// <summary>
        /// IsMatch(): accepts an operation token and identifies if the implemented
        ///		operator class is responsible for it.  This allows for multiple operators
        ///		to represent a given operation (i.e. != both represent the "not-equal" case.
        /// </summary>
        /// <param name="tkn">Parser.Token containing the operator in question</param>
        /// <returns>bool returning true if the object is reponsible for implementing the operator at hand.</returns>
        public abstract bool IsMatch(Parser.Token tkn);

        /// <summary>
        /// Factory(CValue, CValue): responsible for providing an evaluation-time object that
        ///		holds onto the CValue objects it is responsible for operating on.
        /// </summary>
        /// <param name="arg1">First CValue object to operate on</param>
        /// <param name="arg2">Second CValue object to operate on</param>
        /// <returns>"Evaluation time" COperator object</returns>
        public abstract COperator Factory(CValue arg1, CValue arg2);

        /// <summary>
        /// CheckParms( string, CValue, CValue): Helper function that verifies the two arguments
        ///		are non-null objects.  If not, an exception is thrown.
        /// </summary>
        /// <param name="sOp">string representing the operation.</param>
        /// <param name="arg1">CValue object representing the first operation</param>
        /// <param name="arg2">CValue object representing the second oepration</param>
        protected void CheckParms(string sOp, CValue arg1, CValue arg2)
        {
            if (arg1 == null || arg2 == null)
                throw new ApplicationException("Missing expression on " + sOp + " operator.");
        }
    }

    /// <summary>
    /// CFunction class: A CValue derived class that provdes the base for all functions
    ///		implemented in the compiler.  This class also allows for external clients to
    ///		create and register functions with the compiler - thereby extending the compilers
    ///		syntax and functionality.
    /// </summary>
    public abstract class CFunction : CValue
    {
        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public abstract string GetFunction();

        /// <summary>
        /// SetValue():  Accepts an array of CValue objects that represent the parameters in
        ///		the function.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        public void SetValue(List<CValue> alValues)
        {
            m_alValues = alValues;
            CheckParmsInternal();
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected abstract void CheckParmsInternal();

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        ///		that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>Returns a CFunction object that references parameters for evaluation purposes.</returns>
        public abstract CFunction CreateInstance(List<CValue> alValues);

        /// <summary>
        /// An array of values associated with the function.
        /// </summary>
        protected List<CValue> m_alValues;

        /// <summary>
        /// CheckParms(ArrayList, int): Helper function that accepts an array list and insures an appropriate 
        ///		number of CValue objects have been passed. If not, an ApplicationException is thrown.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue-based objects representing parameters to the function</param>
        /// <param name="iRequiredValueCount">integer: required parameter count</param>
        protected void CheckParms(List<CValue> alValues, int iRequiredValueCount)
        {
            if (alValues.Count != iRequiredValueCount)
            {
                string sMsg = string.Format("Invalid parameter count. Function '" + GetFunction() + "' requires {0} parameter(s).", iRequiredValueCount);
                throw new ApplicationException(sMsg);
            }
        }

        /// <summary>
        /// CheckParms(ArrayList, int, int): Helper function that accepts an array list and insures an appropriate
        ///		number (min and/or max) of CValue objects have been passed.  If not an ApplicationException is thrown.  
        /// </summary>
        /// <param name="alValues">ArrayList of CValue object that have been passed by the compiler.</param>
        /// <param name="iMinReq">int value indicating a minimum number of parameters.  -1 if no minimum exists</param>
        /// <param name="iMaxReq">int value indicating a maximum number of parameters.  -1 if no maximum exists</param>
        protected void CheckParms(List<CValue> alValues, int iMinReq, int iMaxReq)
        {
            if (iMinReq > -1)
            {
                if (iMinReq > alValues.Count)
                {
                    string sMsg = string.Format("Invalid parameter count. Function '" + GetFunction() + "' requires a minimum of {0} parameter(s).", iMinReq);
                    throw new ApplicationException(sMsg);
                }
            }

            if (iMaxReq > -1)
            {
                if (iMaxReq < alValues.Count)
                {
                    string sMsg = string.Format("Invalid parameter count. Function '" + GetFunction() + "' is limited to a maximum of {0} parameter(s).", iMaxReq);
                    throw new ApplicationException(sMsg);
                }
            }
        }

        /// <summary>
        /// Sets the execution context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void SetExecutionContext(object context)
        {
            this.Context = context;

            foreach (var item in m_alValues)
            {
                item.SetExecutionContext(context);
            }
        }
    }

    #endregion

}
