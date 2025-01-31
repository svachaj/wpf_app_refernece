using System;
using System.Collections;
using System.Collections.Generic;


namespace dotMath
{
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
    /// 
    /// </remarks>
    /// 
    /// <summary>
    /// CFunctionLibrary provides additional math functions to the 
    /// equation compiler.  All functions may be added to the file using
    /// a static member variable.
    /// </summary>

    public class CFunctionLibrary
    {
        /// <summary>
        /// AddFunctions(EqCompiler): registers all the functions in the library with
        /// the provided compiler instance.
        /// </summary>
        /// <example>
        /// 	<code>
        /// EqCompiler comp = new EqCompiler("sqrt(a+b)");
        /// CFunctionLibrary.AddFunctions( comp );
        /// comp.Compile();
        /// </code>
        /// </example>
        public static void AddFunctions()
        {
            EqCompiler.AddFunction(new CAbs());
            EqCompiler.AddFunction(new CAcos());
            EqCompiler.AddFunction(new CAsin());
            EqCompiler.AddFunction(new CAtan());
            EqCompiler.AddFunction(new CCeiling());
            EqCompiler.AddFunction(new CCos());
            EqCompiler.AddFunction(new CCosh());
            EqCompiler.AddFunction(new CExp());
            EqCompiler.AddFunction(new CFloor());
            EqCompiler.AddFunction(new CLog());
            EqCompiler.AddFunction(new CLog10());
            EqCompiler.AddFunction(new CMax());
            EqCompiler.AddFunction(new CMin());
            EqCompiler.AddFunction(new CRound());
            EqCompiler.AddFunction(new CSign());
            EqCompiler.AddFunction(new CSin());
            EqCompiler.AddFunction(new CSinh());
            EqCompiler.AddFunction(new CSqrt());
            EqCompiler.AddFunction(new CTan());
            EqCompiler.AddFunction(new CTanh());

            EqCompiler.AddFunction(new CIf());

            EqCompiler.AddFunction(new CPower());
            EqCompiler.AddFunction(new CPi());
            EqCompiler.AddFunction(new CRandom());
            EqCompiler.AddFunction(new CRandomInt());
            EqCompiler.AddFunction(new CAvg());
            EqCompiler.AddFunction(new CFrac());
            EqCompiler.AddFunction(new CLogN());
            EqCompiler.AddFunction(new CInt());

            EqCompiler.AddFunction(new CaseFunc());
            EqCompiler.AddFunction(new AnuitniSplatkaUver());
            EqCompiler.AddFunction(new CelkovyVynosFunc());
            EqCompiler.AddFunction(new ContainsFunc());
            EqCompiler.AddFunction(new EqualsFunc());
            EqCompiler.AddFunction(new FalseFunc());
            EqCompiler.AddFunction(new IntervalFunc());
            EqCompiler.AddFunction(new IsEmptyFunc());
            EqCompiler.AddFunction(new IsLowerFunc());
            EqCompiler.AddFunction(new JednotkovyVynosFunc());
            EqCompiler.AddFunction(new ModuloDivFunc());
            EqCompiler.AddFunction(new NotFunc());
            EqCompiler.AddFunction(new Round100Up());
            EqCompiler.AddFunction(new Round10Math());
            EqCompiler.AddFunction(new RoundMath());
            EqCompiler.AddFunction(new SafeDivFunc());
            EqCompiler.AddFunction(new SafeFunc());
            EqCompiler.AddFunction(new StrToIntFunc());
            EqCompiler.AddFunction(new TrueFunc());
            EqCompiler.AddFunction(new TruncateFunc());
        }
    }

    /// <summary>
    ///  CAbs class implements the absolute value (abs(x)) function.
    /// </summary>
    public class CAbs : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CAbs"/> class.
        /// </summary>
        public CAbs()
        {
        }

        /// <summary>
        /// CAbs.CreateInstance returns an instance of the CAbs object
        /// with the passed CValue object(s).
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CAbs oAbs = new CAbs();
            oAbs.SetValue(alValues);

            return oAbs;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        ///  GetValue() is called by the compiler when the user requests the
        ///  function to be evaluated.
        /// </summary>
        /// <returns>
        /// a double value with absolute value applied to the
        /// child parameter
        /// </returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Abs(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction() returns the function name as it appears syntactically
        /// to the compiler.
        /// </summary>
        /// <returns></returns>
        public override string GetFunction()
        {
            return "abs";
        }
    }

    public class CInt : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CAbs"/> class.
        /// </summary>
        public CInt()
        {
        }

        /// <summary>
        /// CAbs.CreateInstance returns an instance of the CAbs object
        /// with the passed CValue object(s).
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CInt cInt = new CInt();
            cInt.SetValue(alValues);

            return cInt;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        ///  GetValue() is called by the compiler when the user requests the
        ///  function to be evaluated.
        /// </summary>
        /// <returns>
        /// a double value with absolute value applied to the
        /// child parameter
        /// </returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            //return (int)((decimal)oValue.GetValue());
            return Convert.ToInt32(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction() returns the function name as it appears syntactically
        /// to the compiler.
        /// </summary>
        /// <returns></returns>
        public override string GetFunction()
        {
            return "int";
        }
    }

    public class CAvg : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CAbs"/> class.
        /// </summary>
        public CAvg()
        {
        }

        /// <summary>
        /// CAbs.CreateInstance returns an instance of the CAbs object
        /// with the passed CValue object(s).
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CAvg oAbs = new CAvg();
            oAbs.SetValue(alValues);

            return oAbs;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2, -1);
        }

        /// <summary>
        ///  GetValue() is called by the compiler when the user requests the
        ///  function to be evaluated.
        /// </summary>
        /// <returns>
        /// a double value with absolute value applied to the
        /// child parameter
        /// </returns>
        public override double GetValue()
        {
            double dValue = 0;

            for (int i = 0; i < m_alValues.Count; i++)
            {
                dValue += m_alValues[i].GetValue();
            }

            dValue = dValue / m_alValues.Count;

            return dValue;
        }

        /// <summary>
        /// GetFunction() returns the function name as it appears syntactically
        /// to the compiler.
        /// </summary>
        /// <returns></returns>
        public override string GetFunction()
        {
            return "avg";
        }
    }

    /// <summary>
    /// CAcos implements calculating the angle whose cosine is the specified number.
    /// </summary>
    public class CAcos : CFunction
    {

        /// <summary>
        /// Standard Constructor - used for instantiating object for creation
        /// of other CAcos objects.
        /// </summary>
        public CAcos()
        {
        }

        /// <summary>
        /// Creates an instance of CAcos used in the expression compilation. 
        /// </summary>
        /// <param name="alValues">array list of CValue objects representing
        /// the indicated child-values</param>
        /// <returns>CFunction-derived object - CAcos</returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CAcos oAcos = new CAcos();
            oAcos.SetValue(alValues);

            return oAcos;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }


        /// <summary>
        /// GetValue returns a double value as a result of running the
        /// function against child-CValue objects.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = m_alValues[0];
            return Math.Acos(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction() returns the function name according to the syntax
        /// of the function within the compiler.
        /// </summary>
        /// <returns>string containing the function name</returns>
        public override string GetFunction()
        {
            return "acos";
        }
    }


    /// <summary>
    /// CAsin implements calculating the angle whose sine is the specified number.
    /// </summary>
    public class CAsin : CFunction
    {
        /// <summary>
        /// Simple constructor.  Used when registering with the equation compiler.
        /// </summary>
        public CAsin()
        {
        }

        /// <summary>
        /// CreateInstance() is responsible for createing a new instance
        /// of this object that can be used in calculations.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>CFunction</returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CAsin oAsin = new CAsin();
            oAsin.SetValue(alValues);

            return oAsin;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// GetValue() returns the value of the object and it's child members.
        /// </summary>
        /// <returns>double representing the value in the object.</returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Asin(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction() : returns the name of the function.
        /// </summary>
        /// <returns></returns>
        public override string GetFunction()
        {
            return "asin";
        }
    }


    /// <summary>
    /// CAtan implements calculating the angle whose tangent is the specified number.
    /// </summary>
    public class CAtan : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CAtan"/> class.
        /// </summary>
        public CAtan()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CAtan oAtan = new CAtan();
            oAtan.SetValue(alValues);

            return oAtan;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Atan(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "atan";
        }
    }

    /// <summary>
    /// CFloor class: Returns the largest whole number less than or equal to the specified number.
    /// </summary>
    public class CFloor : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CFloor"/> class.
        /// </summary>
        public CFloor()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CFloor oFloor = new CFloor();
            oFloor.SetValue(alValues);

            return oFloor;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Floor(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "floor";
        }
    }



    /// <summary>
    /// CCeiling class: Returns the largest whole number greater than or equal to the specified number.
    /// </summary>
    public class CCeiling : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CCeiling"/> class.
        /// </summary>
        public CCeiling()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CCeiling oCeiling = new CCeiling();
            oCeiling.SetValue(alValues);

            return oCeiling;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Ceiling(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "ceiling";
        }
    }

    /// <summary>
    /// CExp class: Returns e raised to the specified power.
    /// </summary>
    public class CExp : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CExp"/> class.
        /// </summary>
        public CExp()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CExp oExp = new CExp();
            oExp.SetValue(alValues);

            return oExp;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Exp(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "exp";
        }
    }



    /// <summary>
    /// CSin class: returns the sine of the specified angle.
    /// </summary>
    public class CSin : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSin"/> class.
        /// </summary>
        public CSin()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CSin oSin = new CSin();
            oSin.SetValue(alValues);

            return oSin;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            return Math.Sin(m_alValues[0].GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "sin";
        }
    }


    /// <summary>
    /// CSinh class: Returns the hyperbolic sine of the specified angle.
    /// </summary>
    public class CSinh : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSinh"/> class.
        /// </summary>
        public CSinh()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CSinh oSinh = new CSinh();
            oSinh.SetValue(alValues);

            return oSinh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue1 = (CValue)m_alValues[0];
            return Math.Sinh(oValue1.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "sinh";
        }
    }

    /// <summary>
    /// CSqrt class: implements the square root function.
    /// </summary>
    public class CSqrt : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSqrt"/> class.
        /// </summary>
        public CSqrt()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CSqrt oSqrt = new CSqrt();
            oSqrt.SetValue(alValues);

            return oSqrt;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue1 = (CValue)m_alValues[0];
            return Math.Sqrt(oValue1.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "sqrt";
        }
    }

    /// <summary>
    /// CCos class: returns the cosine of the specified angle
    /// </summary>
    public class CCos : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CCos"/> class.
        /// </summary>
        public CCos()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CCos oCos = new CCos();
            oCos.SetValue(alValues);

            return oCos;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Cos(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "cos";
        }
    }



    /// <summary>
    /// CCosh class: returns the hyperbolic cosine of the specified angle
    /// </summary>
    public class CCosh : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CCosh"/> class.
        /// </summary>
        public CCosh()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CCosh oCos = new CCosh();
            oCos.SetValue(alValues);

            return oCos;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Cosh(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "cosh";
        }
    }


    /// <summary>
    /// CIf class: implements if(condition, then,else) functionality.
    /// </summary>
    public class CIf : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CIf"/> class.
        /// </summary>
        public CIf()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CIf oIf = new CIf();
            oIf.SetValue(alValues);

            return oIf;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 3);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oTest = (CValue)m_alValues[0];
            CValue oThen = (CValue)m_alValues[1];
            CValue oElse = (CValue)m_alValues[2];

            if (oTest.GetValue() != 0.0)
                return oThen.GetValue();
            else
                return oElse.GetValue();

        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "if";
        }
    }

    //	/// <summary>
    //	/// CCase class: implements case(condition1, then, condition2, then,... else) functionality.
    //	/// </summary>
    //	public class CCase : CFunction
    //	{
    //		public CCase()
    //		{
    //		}
    //
    //		public override CFunction CreateInstance( List<CValue> alValues )
    //		{
    //			CCase oCase = new CCase();
    //			oCase.SetValue( alValues );
    //
    //			return oCase;
    //		}
    //
    //		public override void SetValue( List<CValue> alValues )
    //		{
    //			//CheckParms( alValues, 3);
    //			m_alValues = alValues;
    //		}
    //
    //		public override double GetValue()
    //		{
    //			CValue oTest = (CValue) m_alValues[0];
    //			CValue oThen = (CValue) m_alValues[1];
    //			CValue oElse = (CValue) m_alValues[2];
    //
    //			if( oTest.GetValue() != 0.0 )
    //				return oThen.GetValue();
    //			else
    //				return oElse.GetValue();
    //
    //		}
    //
    //		public override string GetFunction()
    //		{
    //			return "case";
    //		}
    //	}

    /// <summary>
    /// CMax class: returns the maximum value among provided parameters
    /// </summary>
    public class CMax : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CMax"/> class.
        /// </summary>
        public CMax()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CMax oMax = new CMax();
            oMax.SetValue(alValues);

            return oMax;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2, -1);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            double dValue = oValue.GetValue();

            for (int i = 1; i < m_alValues.Count; i++)
            {
                oValue = (CValue)m_alValues[i];

                dValue = Math.Max(dValue, oValue.GetValue());
            }

            return dValue;



        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "max";
        }
    }


    /// <summary>
    /// CRound class:  implements a rouding of a number to the nearest whole number.
    /// </summary>
    public class CRound : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CRound"/> class.
        /// </summary>
        public CRound()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CRound oRound = new CRound();
            oRound.SetValue(alValues);

            return oRound;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            double decs = m_alValues[1].GetValue();

            decimal newVal = (decimal)oValue.GetValue();

            double result = (double)Math.Round(newVal, (int)decs, MidpointRounding.AwayFromZero);
                      
            return result;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "round";
        }
    }

    /// <summary>
    /// power function
    /// </summary>
    public class CPower : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CRound"/> class.
        /// </summary>
        public CPower()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CPower oRound = new CPower();
            oRound.SetValue(alValues);

            return oRound;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            double x = m_alValues[0].GetValue();
            double y = m_alValues[1].GetValue();

            return Math.Pow(x, y);
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "power";
        }
    }


    /// <summary>
    /// PI function
    /// </summary>
    public class CPi : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CRound"/> class.
        /// </summary>
        public CPi()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CPi oRound = new CPi();
            oRound.SetValue(alValues);

            return oRound;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 0);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {

            return Math.PI;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "pi";
        }
    }


    /// <summary>
    /// random function
    /// </summary>
    public class CRandom : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CRound"/> class.
        /// </summary>
        public CRandom()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CRandom oRound = new CRandom();
            oRound.SetValue(alValues);

            return oRound;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 0);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            Random rnd = new Random();

            return rnd.NextDouble();
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "random";
        }
    }


    public class CRandomInt : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CRound"/> class.
        /// </summary>
        public CRandomInt()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CRandomInt oRound = new CRandomInt();
            oRound.SetValue(alValues);

            return oRound;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            Random rnd = new Random();
            int rndint = (int)m_alValues[0].GetValue();

            return rnd.Next(rndint);
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "randomint";
        }
    }



    public class CFrac : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSign"/> class.
        /// </summary>
        public CFrac()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CFrac oSign = new CFrac();
            oSign.SetValue(alValues);

            return oSign;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);  // check for a minimum of two values (no maximum limit)
        }


        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            double oValue = m_alValues[0].GetValue();
            double retd = oValue - Math.Floor(oValue);

            return retd;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "frac";
        }
    }



    /// <summary>
    /// CSign class: returns a value that indicates the sign of the provide value.
    ///	Returns the following possibilties:
    ///	value les then 0 = -1
    ///	value = 0 = 0
    ///	value greater then 0 = 1
    /// </summary>
    public class CSign : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSign"/> class.
        /// </summary>
        public CSign()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CSign oSign = new CSign();
            oSign.SetValue(alValues);

            return oSign;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);  // check for a minimum of two values (no maximum limit)
        }


        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Sign(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "sign";
        }
    }

    /// <summary>
    /// CMin class:  returns the minimum value among supplied values.
    /// </summary>
    public class CMin : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CMin"/> class.
        /// </summary>
        public CMin()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CMin oMin = new CMin();
            oMin.SetValue(alValues);

            return oMin;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2, -1);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            double dValue = oValue.GetValue();

            for (int i = 1; i < m_alValues.Count; i++)
            {
                oValue = (CValue)m_alValues[i];

                dValue = Math.Min(dValue, oValue.GetValue());
            }

            return dValue;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "min";
        }
    }



    /// <summary>
    /// CTan class: return the tagent of the supplied angle
    /// </summary>
    public class CTan : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CTan"/> class.
        /// </summary>
        public CTan()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CTan oTan = new CTan();
            oTan.SetValue(alValues);

            return oTan;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Tan(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "tan";
        }
    }

    /// <summary>
    /// CLog class: returns the logarithm of a specified number.
    /// </summary>
    public class CLog : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CLog"/> class.
        /// </summary>
        public CLog()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CLog oLog = new CLog();
            oLog.SetValue(alValues);

            return oLog;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Log(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "log";
        }
    }


    /// <summary>
    /// CLog10 class:  Returns the base 10 logarithm of a specified number.
    /// </summary>
    public class CLog10 : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CLog10"/> class.
        /// </summary>
        public CLog10()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CLog10 oLog10 = new CLog10();
            oLog10.SetValue(alValues);

            return oLog10;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Log10(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "log10";
        }
    }

    /// <summary>
    /// logn function
    /// </summary>
    public class CLogN : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CLog10"/> class.
        /// </summary>
        public CLogN()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CLogN oLogN = new CLogN();
            oLogN.SetValue(alValues);

            return oLogN;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            CValue oValue2 = (CValue)m_alValues[1];
            return Math.Log(oValue.GetValue(), oValue2.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "logn";
        }
    }

    /// <summary>
    /// CTanh class: Returns the hyperbolic tangent of the supplied angle.
    /// </summary>
    public class CTanh : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CTanh"/> class.
        /// </summary>
        public CTanh()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CTanh oTanh = new CTanh();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);  // check for a minimum of two values (no maximum limit)
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Tanh(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "tanh";
        }
    }

    /// <summary>
    /// Case function
    /// </summary>
    public class CaseFunc : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaseFunc"/> class.
        /// </summary>
        public CaseFunc()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CaseFunc oTanh = new CaseFunc();
            oTanh.SetValue(alValues);
            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            if (m_alValues.Count < 3)
            {
                string sMsg = string.Format("Invalid parameter count. Function '" + GetFunction() + "' requires min 3 parameter(s).");
                throw new ApplicationException(sMsg);
            }
            if (m_alValues.Count % 2 == 0)
            {
                string sMsg = string.Format("Invalid parameter count. Function '" + GetFunction() + "' requires even parameters count.");
                throw new ApplicationException(sMsg);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            bool isEven = false;
            bool isCase = false;
            double ret;

            for (int i = 0; i < m_alValues.Count; i++)
            {
                if (!isEven)
                {
                    string value = ((CValue)m_alValues[i]).GetValueString();
                    double dblValue = ((CValue)m_alValues[i]).GetValue();
                    if (i == m_alValues.Count - 1)
                    {
                        ret = ((CValue)m_alValues[i]).GetValue();
                        return ret;
                        //isCase = true;
                        //isEven = true;
                        //continue;
                    }
                    if (dblValue != 0)
                    {
                        isCase = true;
                        isEven = true;
                        continue;
                    }
                    isEven = true;
                    continue;
                }
                if (isEven)
                {
                    ret = ((CValue)m_alValues[i]).GetValue();
                    if (isCase)
                    {
                        return ret;
                    }
                    else
                    {
                        isEven = false;
                    }
                }
            }
            throw new ApplicationException("Invalid code flow on CASE() function");
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "case";
        }
    }


    public class Round10Math : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Round10Math"/> class.
        /// </summary>
        public Round10Math()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            Round10Math oTanh = new Round10Math();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Round((int)oValue.GetValue() * 0.1, 0) * 10;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "round10math";
        }
    }

    /// <summary>
    /// RoundMath class
    /// </summary>
    public class RoundMath : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundMath"/> class.
        /// </summary>
        public RoundMath()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            RoundMath oTanh = new RoundMath();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            CValue oValueR = (CValue)m_alValues[1];

            return (double)Round((decimal)oValue.GetValue(), (int)oValueR.GetValue());
            //return Math.Round(oValue.GetValue(), (int)oValueR.GetValue());
        }

        /// <summary>
        /// Zaokrouhlení matematicke
        /// </summary>
        /// <param name="avalue">The avalue.</param>
        /// <param name="decimals">The decimals.</param>
        /// <returns></returns>
        public static decimal Round(decimal avalue, int decimals)
        {
            if (decimals >= 0)
                return Math.Round(avalue, decimals);
            else
            {
                decimals = Math.Abs(decimals);
                decimal delitel = (decimal)Math.Pow(10, decimals);
                //				for( int i=1; i<=decimals; i++ )
                //					delitel *= 10m;
                return Math.Round(avalue / delitel, 0) * delitel;
            }
        }


        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "roundmath";
        }
    }

    /// <summary>
    /// Round10Math class: Returns the hyperbolic tangent of the supplied angle.
    /// </summary>
    public class Round100Up : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Round100Up"/> class.
        /// </summary>
        public Round100Up()
        {
        }
        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            Round100Up oTanh = new Round100Up();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return MakeRound100Up(oValue.GetValue());
        }

        /// <summary>
        /// Zaokrouhlení na 100 nahoru
        /// </summary>
        /// <param name="avalue"></param>
        /// <returns></returns>
        public static double MakeRound100Up(double avalue)
        {
            int PTrunc = (int)avalue;
            double Temp = ((PTrunc * 0.01) - (int)(PTrunc * 0.01)) * 100;
            Temp = PTrunc - Temp;
            if (avalue != Temp) { Temp = Temp + 100; }

            return (double)(int)Temp;
        }


        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "round100up";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TrueFunc : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrueFunc"/> class.
        /// </summary>
        public TrueFunc()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            TrueFunc oTanh = new TrueFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return oValue.GetValue() != 0 ? 1 : 0;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "true";
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class FalseFunc : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FalseFunc"/> class.
        /// </summary>
        public FalseFunc()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            FalseFunc oTanh = new FalseFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return oValue.GetValue() == 0 ? 1 : 0;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "false";
        }
    }

    /// <summary>
    /// Round10Math class: Returns the hyperbolic tangent of the supplied angle.
    /// </summary>
    public class IntervalFunc : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalFunc"/> class.
        /// </summary>
        public IntervalFunc()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            IntervalFunc oTanh = new IntervalFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 3);
        }


        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue1 = (CValue)m_alValues[0];
            CValue oValue2 = (CValue)m_alValues[1];
            CValue oValue3 = (CValue)m_alValues[2];
            return (double)Interval((decimal)oValue1.GetValue(), (decimal)oValue2.GetValue(), (decimal)oValue3.GetValue());
        }

        /// <summary>
        /// Vrátí pøedanou hodnotu jestliže se nahází uvnitø daného intervalu, jinak vrátí hranièní hodnotu
        /// </summary>
        /// <param name="min">min hodnota</param>
        /// <param name="max">max hodnota</param>
        /// <param name="val">zkoumaná hodnota</param>
        /// <returns></returns>
        public static decimal Interval(decimal min, decimal max, decimal val)
        {
            return Math.Max(Math.Min(max, val), min);
        }


        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "interval";
        }
    }

    /// <summary>
    /// KumulativniVynosFunc class: vrátí celkový výnos v letech
    /// </summary>
    /// <example>
    /// CelkovyVynos( 0.02, 5 );
    /// Spoèítá celkový výnos za pìt let pøi výnosu 2% p.a. = 0,10408 --> cca 10%
    /// </example>
    public class CelkovyVynosFunc : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CelkovyVynosFunc"/> class.
        /// </summary>
        public CelkovyVynosFunc()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            CelkovyVynosFunc oTanh = new CelkovyVynosFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue1 = (CValue)m_alValues[0];
            CValue oValue2 = (CValue)m_alValues[1];
            return (double)Math.Pow(1f + oValue1.GetValue(), oValue2.GetValue()) - 1f;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "celkovyvynos";
        }
    }
    /// <summary>
    /// JednotkovyVynosFunc class: jednotkový výnos v letech pøi zadaném celkovém zhodnocení
    /// </summary>
    /// <example>
    /// CelkovyVynos( 0.10408, 5 );
    /// Spoèítá jednotkový úrok za pìt let = 0,02 --> 2%
    /// </example>
    public class JednotkovyVynosFunc : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JednotkovyVynosFunc"/> class.
        /// </summary>
        public JednotkovyVynosFunc()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            JednotkovyVynosFunc oTanh = new JednotkovyVynosFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue1 = (CValue)m_alValues[0];
            CValue oValue2 = (CValue)m_alValues[1];
            return (double)Math.Pow(1f + oValue1.GetValue(), 1f / oValue2.GetValue()) - 1f;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "jednotkovyvynos";
        }
    }





    /// <summary>
    /// Reprezentuje anuitni splatku pro dany uver pri zadane mesicni urokove sazbe a poctu mesicu
    /// </summary>
    public class AnuitniSplatkaUver : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnuitniSplatkaUver"/> class.
        /// </summary>
        public AnuitniSplatkaUver()
        {
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="alValues">The al values.</param>
        /// <returns></returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            AnuitniSplatkaUver oTanh = new AnuitniSplatkaUver();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 3);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue vyseUveru = (CValue)m_alValues[0];
            CValue mesicniUrokovaSazba = (CValue)m_alValues[1];
            CValue pocetMesicuSplaceni = (CValue)m_alValues[1];

            if (mesicniUrokovaSazba.GetValue() != 0)
            {
                return (vyseUveru.GetValue() * mesicniUrokovaSazba.GetValue()) / (double)(1 - (Math.Pow((double)(1 / (1 + mesicniUrokovaSazba.GetValue())), (double)pocetMesicuSplaceni.GetValue())));
            }
            else
            {
                return vyseUveru.GetValue() / pocetMesicuSplaceni.GetValue();
            }


        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "anuitnisplatkauver";
        }
    }

    ///// <summary>
    ///// Pøevod mìn
    ///// </summary>
    //public class ExchangeCurrencyFunc : CFunction
    //{

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ExchangeCurrencyFunc"/> class.
    //    /// </summary>
    //    public ExchangeCurrencyFunc()
    //    {
    //    }

    //    /// <summary>
    //    /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
    //    /// that performs the operation on a set of CValue objects.
    //    /// </summary>
    //    /// <param name="alValues">ArrayList of CValue parameter objects.</param>
    //    /// <returns>
    //    /// Returns a CFunction object that references parameters for evaluation purposes.
    //    /// </returns>
    //    public override CFunction CreateInstance(List<CValue> alValues)
    //    {
    //        ExchangeCurrencyFunc oTanh = new ExchangeCurrencyFunc();
    //        oTanh.SetValue(alValues);

    //        return oTanh;
    //    }

    //    /// <summary>
    //    /// Gets the value.
    //    /// </summary>
    //    /// <returns></returns>
    //    public override double GetValue()
    //    {
    //        double val = (m_alValues[0] as dotMath.CValue).GetValue();
    //        string c1 = (m_alValues[1] as dotMath.CValue).GetValueString();
    //        string c2 = (m_alValues[2] as dotMath.CValue).GetValueString();

    //        (this.Context as ExpressionEvaluator).Service.E
    //        return Convert.ToDouble(CreaSoft.Optim.Common.Currency.Exchange((decimal)val, c1, c2));
    //    }

    //    public override string GetFunction()
    //    {
    //        return "exchangecurrency";
    //    }
    //}


    /// <summary>
    /// Bezpeèné dìlení nulou
    /// </summary>
    public class SafeDivFunc : CFunction
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public SafeDivFunc()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            SafeDivFunc oTanh = new SafeDivFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            double oValue1 = ((CValue)m_alValues[0]).GetValue();
            double oValue2 = ((CValue)m_alValues[1]).GetValue();
            if (oValue2 == 0)
                return 0;
            else
                return oValue1 / oValue2;

        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "safediv";
        }
    }

    /// <summary>
    /// Bezpeèné volání výrazu - ošetøí výjimku dìlení nulou nebo výsledek typu float.NaN
    /// </summary>
    public class SafeFunc : CFunction
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public SafeFunc()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            SafeFunc oTanh = new SafeFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            double ret = ((CValue)m_alValues[0]).GetValue();
            if (double.IsNaN(ret))
                ret = 0f;
            return ret;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "safe";
        }
    }

    /// <summary>
    /// Is Empty string
    /// </summary>
    public class IsEmptyFunc : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsEmptyFunc"/> class.
        /// </summary>
        public IsEmptyFunc()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            IsEmptyFunc oTanh = new IsEmptyFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            string oValue1 = ((CValue)m_alValues[0]).GetValueString();

            if (oValue1 == string.Empty)
                return 1;
            else
                return 0;

        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "isempty";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EqualsFunc : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualsFunc"/> class.
        /// </summary>
        public EqualsFunc()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            EqualsFunc oTanh = new EqualsFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            string oValue1 = ((CValue)m_alValues[0]).GetValueString();
            string oValue2 = ((CValue)m_alValues[1]).GetValueString();
            if (oValue1 == oValue2)
                return 1;
            else
                return 0;

        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "equals";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ContainsFunc : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainsFunc"/> class.
        /// </summary>
        public ContainsFunc()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            ContainsFunc oTanh = new ContainsFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            string oValue1 = ((CValue)m_alValues[0]).GetValueString();
            string oValue2 = ((CValue)m_alValues[1]).GetValueString();

            //foreach (string product in from s in oValue1.Split(',')
            //                           where !string.IsNullOrEmpty(s)
            //                           select s.Trim()
            //                               )
            //{
            //    if (product == oValue2)
            //        return 1;
            //}

            foreach (string str in oValue1.Split(','))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    if (str.Trim() == oValue2)
                        return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "contains";
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ModuloDivFunc : CFunction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuloDivFunc"/> class.
        /// </summary>
        public ModuloDivFunc()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            ModuloDivFunc oTanh = new ModuloDivFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            double oValue1 = ((CValue)m_alValues[0]).GetValue();
            double oValue2 = ((CValue)m_alValues[1]).GetValue();
            if (oValue2 == 0 || (oValue2 > oValue1))
                return 0;
            else
            {
                oValue1 -= oValue1 % oValue2;
                return oValue1 / oValue2;
            }

        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "modulodiv";
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class NotFunc : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFunc"/> class.
        /// </summary>
        public NotFunc()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            NotFunc oTanh = new NotFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }


        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            double oValue1 = ((CValue)m_alValues[0]).GetValue();
            if (oValue1 == 0)
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "not";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class IsLowerFunc : CFunction
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="IsLowerFunc"/> class.
        /// </summary>
        public IsLowerFunc()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            IsLowerFunc oTanh = new IsLowerFunc();
            oTanh.SetValue(alValues);
            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 2);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            double oValue1 = ((CValue)m_alValues[0]).GetValue();
            double oValue2 = ((CValue)m_alValues[1]).GetValue();
            if (oValue1 < oValue2)
                return 1;
            else
                return 0;

        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "islower";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StrToIntFunc : CFunction
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="StrToIntFunc"/> class.
        /// </summary>
        public StrToIntFunc()
        {
        }


        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            StrToIntFunc oTanh = new StrToIntFunc();
            oTanh.SetValue(alValues);
            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            string oValue1 = ((CValue)m_alValues[0]).GetValueString();
            if (oValue1 != string.Empty)
                return System.Convert.ToInt32(oValue1);
            else
                return 0;
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "strtoint";
        }
    }

    /// <summary>
    /// truncate specifed number
    /// </summary>
    public class TruncateFunc : CFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Round10Math"/> class.
        /// </summary>
        public TruncateFunc()
        {
        }

        /// <summary>
        /// CreateInstance( ArrayList ):  Requests that an evaluation-time object be created
        /// that performs the operation on a set of CValue objects.
        /// </summary>
        /// <param name="alValues">ArrayList of CValue parameter objects.</param>
        /// <returns>
        /// Returns a CFunction object that references parameters for evaluation purposes.
        /// </returns>
        public override CFunction CreateInstance(List<CValue> alValues)
        {
            TruncateFunc oTanh = new TruncateFunc();
            oTanh.SetValue(alValues);

            return oTanh;
        }

        /// <summary>
        /// Checks the parms internal.
        /// </summary>
        protected override void CheckParmsInternal()
        {
            CheckParms(m_alValues, 1);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            CValue oValue = (CValue)m_alValues[0];
            return Math.Truncate(oValue.GetValue());
        }

        /// <summary>
        /// GetFunction():  returns the function name as a string
        /// </summary>
        /// <returns>string</returns>
        public override string GetFunction()
        {
            return "truncate";
        }
    }

}
