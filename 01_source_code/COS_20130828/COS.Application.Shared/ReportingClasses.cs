using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using COS.Resources;
using System.Windows;
using System.Reflection.Emit;
using System.Reflection;

namespace COS.Application.Shared
{
    public class KPIReportData : ValidationViewModelBase
    {
        public KPIReportData()
            : base()
        {
            ItemsCount = 1;

        }

        public KPIReportData(List<WorkCenter> wcs)
            : base()
        {
            ItemsCount = 1;

            GenerateDictionaries(wcs);

        }

        private void GenerateDictionaries(List<WorkCenter> wcs)
        {
            if (HlpPerformanceDict != null)
                HlpPerformanceDict.Clear();
            else
                HlpPerformanceDict = new Dictionary<string, decimal>();

            if (HlpProductivityDict != null)
                HlpProductivityDict.Clear();
            else
                HlpProductivityDict = new Dictionary<string, decimal>();


            if (ActualPcsPerHeadhourDict != null)
                ActualPcsPerHeadhourDict.Clear();
            else
                ActualPcsPerHeadhourDict = new Dictionary<string, decimal>();


            if (ItemsCountDict != null)
                ItemsCountDict.Clear();
            else
                ItemsCountDict = new Dictionary<string, int>();


            if (KpiActualTimeDict != null)
                KpiActualTimeDict.Clear();
            else
                KpiActualTimeDict = new Dictionary<string, int>();


            if (KpiProducedPcsDict != null)
                KpiProducedPcsDict.Clear();
            else
                KpiProducedPcsDict = new Dictionary<string, decimal>();

            if (ScrapPcsDict != null)
                ScrapPcsDict.Clear();
            else
                ScrapPcsDict = new Dictionary<string, int>();

            if (OperationalTimeDict != null)
                OperationalTimeDict.Clear();
            else
                OperationalTimeDict = new Dictionary<string, decimal>();



            foreach (var wc in wcs)
            {
                HlpPerformanceDict.Add(wc.Value, 0);
                HlpProductivityDict.Add(wc.Value, 0);
                ActualPcsPerHeadhourDict.Add(wc.Value, 0);
                ItemsCountDict.Add(wc.Value, 1);
                KpiActualTimeDict.Add(wc.Value, 0);
                KpiProducedPcsDict.Add(wc.Value, 0);
                ScrapPcsDict.Add(wc.Value, 0);
                OperationalTimeDict.Add(wc.Value, 0);
            }
        }

        public decimal KpiPerformance
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();


                values.Add("OperationalTime", (double)OperationalTime);
                values.Add("hlpPerformance", (double)HlpPerformance);


                result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiPerformance", values), 2);


                return result;
            }
        }

        public decimal KpiPerformanceC(string key)
        {
            decimal result = 0;

            Dictionary<string, double> values = new Dictionary<string, double>();

            values.Add("OperationalTime", (double)OperationalTimeDict[key]);
            values.Add("hlpPerformance", (double)HlpPerformanceDict[key]);


            result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiPerformance", values), 2);


            return result;

        }


        public decimal KpiProductivity
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("OperationalTime", (double)OperationalTime);
                values.Add("hlpProductivity", (double)HlpProductivity);



                result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiProductivity", values), 2);


                return result;
            }
        }

        public decimal KpiHRProductivity
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("OperationalTime", (double)OperationalTime);
                values.Add("hlpProductivity", (double)HlpHRProductivity);



                result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiProductivity", values), 2);


                return result;
            }
        }

        public decimal KpiProductivityC(string key)
        {
            decimal result = 0;

            Dictionary<string, double> values = new Dictionary<string, double>();

            values.Add("OperationalTime", (double)OperationalTimeDict[key]);
            values.Add("hlpProductivity", (double)HlpProductivityDict[key]);



            result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiProductivity", values), 2);


            return result;
        }

        public int ItemsCount { set; get; }

        public Dictionary<string, int> ItemsCountDict { set; get; }

        public decimal KpiAvailability
        {
            get
            {

                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("ActualTime", (double)KpiActualTime);
                values.Add("OperationalTime", (double)OperationalTime);

                result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiAvailability", values), 2);


                return result;
            }
        }

        public decimal KpiAvailabilityC(string key)
        {
            decimal result = 0;

            Dictionary<string, double> values = new Dictionary<string, double>();

            values.Add("ActualTime", (double)KpiActualTimeDict[key]);
            values.Add("OperationalTime", (double)OperationalTimeDict[key]);

            result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiAvailability", values), 2);


            return result;

        }


        //private Dictionary<string, decimal> _SuperValues = new Dictionary<string, decimal>();

        //public Dictionary<string, decimal> SuperValues 
        //{
        //    set 
        //    {
        //        _SuperValues = value;
        //        OnPropertyChanged("SuperValues");
        //    }
        //    get 
        //    {
        //        return _SuperValues;
        //    }
        //}


        public KPIReportData Add<T>(string key, T value)
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Dynamic.dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(Guid.NewGuid().ToString());
            typeBuilder.SetParent(this.GetType());
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(key, PropertyAttributes.None, typeof(T), Type.EmptyTypes);

            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + key, MethodAttributes.Public, CallingConventions.HasThis, typeof(T), Type.EmptyTypes);
            ILGenerator getter = getMethodBuilder.GetILGenerator();
            getter.Emit(OpCodes.Ldarg_0);
            getter.Emit(OpCodes.Ldstr, key);
            getter.Emit(OpCodes.Callvirt, typeof(KPIReportData).GetMethod("Get", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(typeof(T)));
            getter.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getMethodBuilder);

            Type type = typeBuilder.CreateType();

            KPIReportData child = (KPIReportData)Activator.CreateInstance(type);
            child.dictionary = this.dictionary;
            dictionary.Add(key, value);
            return child;
        }

        protected T Get<T>(string key)
        {
            return (T)dictionary[key];
        }

        public object this[string key]
        {
            get { return dictionary.ContainsKey(key) ? dictionary[key] : null; }

            set
            {
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key] = value;
                }
                else
                {
                    dictionary.Add(key, value);
                }
            }
        }

        private Dictionary<string, object> dictionary = new Dictionary<string, object>();

        private List<decimal> _SuperValues = new List<decimal>();

        public List<decimal> SuperValues
        {
            set
            {
                _SuperValues = value;
                OnPropertyChanged("SuperValues");
            }
            get
            {
                return _SuperValues;
            }
        }


        public decimal KpiQuality
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("ProducedPcs", (double)KpiProducedPcs);
                values.Add("ScrapPcs", ScrapPcs);

                result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiQuality", values), 2);


                return result;
            }
        }

        public decimal KpiQualityC(string key)
        {
            decimal result = 0;

            Dictionary<string, double> values = new Dictionary<string, double>();

            values.Add("ProducedPcs", (double)KpiProducedPcsDict[key]);
            values.Add("ScrapPcs", ScrapPcsDict[key]);

            result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiQuality", values), 2);


            return result;

        }

        private decimal _kpiOEE = 0;
        public decimal KpiOEE
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("ReportKpiQuality", (double)KpiQuality);
                values.Add("ReportKpiPerformance", (double)KpiPerformance);
                values.Add("ReportKpiAvailability", (double)KpiAvailability);


                result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiOEE", values), 2);


                return result;
            }
        }

        public decimal KpiOEEC(string key)
        {
            decimal result = 0;

            Dictionary<string, double> values = new Dictionary<string, double>();

            values.Add("ReportKpiQuality", (double)KpiQualityC(key));
            values.Add("ReportKpiPerformance", (double)KpiPerformanceC(key));
            values.Add("ReportKpiAvailability", (double)KpiAvailabilityC(key));


            result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiOEE", values), 2);


            return result;
        }


        private int _kpiActualTime = 0;
        public int KpiActualTime
        {
            set
            {
                _kpiActualTime = value;
                OnPropertyChanged("KpiActualTime");
            }
            get
            {
                return _kpiActualTime;
            }
        }
        public Dictionary<string, int> KpiActualTimeDict { set; get; }

        private decimal _kpiProducedPcs = 0;
         public decimal KpiProducedPcs
        {
            set
            {
                _kpiProducedPcs = value;
                OnPropertyChanged("KpiProducedPcs");
            }
            get
            {
                return _kpiProducedPcs;
            }
        }
        public Dictionary<string, decimal> KpiProducedPcsDict { set; get; }

        public decimal ActualPcsPerHeadhour
        {
            set;
            get;
        }

        public decimal ActualPcsPerHeadhourOperTime
        {
            set;
            get;
        }

        public Dictionary<string, decimal> ActualPcsPerHeadhourDict { set; get; }



        public decimal KpiActualPcsPerHeadhour
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("OperationalTime", (double)OperationalTime);
                values.Add("SumActPiecesPerHeadHourOperationalTime", (double)ActualPcsPerHeadhourOperTime);


                result = Math.Round((decimal)Calculation.CalculateFunction("ReportActPcsPerHeadHour", values), 2);


                return result;
            }
        }


        public decimal SumActPiecesPerHeadHourOperationalTime
        {
            get
            {
                return OperationalTime == 0 ? 0 : ActualPcsPerHeadhour / ItemsCount * OperationalTime;
            }
        }


        public decimal HlpPerformance { set; get; }
        public Dictionary<string, decimal> HlpPerformanceDict { set; get; }


        public decimal HlpHRProductivity { set; get; }

        public decimal HlpProductivity { set; get; }
        public Dictionary<string, decimal> HlpProductivityDict { set; get; }

        public int ScrapPcs { set; get; }
        public Dictionary<string, int> ScrapPcsDict { set; get; }


        public decimal OperationalTime
        {
            set;
            get;
        }
        public Dictionary<string, decimal> OperationalTimeDict { set; get; }


        public decimal TargetPlan
        {
            get
            {
                decimal result = 0;

                var rt = COSContext.Current.ReportTargets.FirstOrDefault(a => a.ID_Division == IdDivision &&
                    a.Code == ActualCode && a.ReportType == "OEE1" && a.Year == Date.Value.Year);

                if (rt != null)
                {
                    result = rt.Value;
                }

                return result;
            }
        }


        public Style ImageOEE
        {
            get
            {
                Style style = ResourceHelper.GetResource<Style>("StatusPointColor1");


                decimal deviation = TargetOEE - (TargetOEE * (decimal)0.08);

                if (KpiOEE >= TargetOEE)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor1");
                }
                else if (KpiOEE >= deviation)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor2");
                }
                else if (KpiOEE > 0)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor3");
                }
                else
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor4");
                }

                return style;
            }
        }

        public Style ImagePerformance
        {
            get
            {
                Style style = ResourceHelper.GetResource<Style>("StatusPointColor1");


                decimal deviation = TargetPerformance - (TargetPerformance * (decimal)0.08);

                if (KpiPerformance >= TargetPerformance)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor1");
                }
                else if (KpiPerformance >= deviation)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor2");
                }
                else if (KpiPerformance > 0)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor3");
                }
                else
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor4");
                }

                return style;
            }
        }

        public Style ImageQuality
        {
            get
            {
                Style style = ResourceHelper.GetResource<Style>("StatusPointColor1");


                decimal deviation = TargetQuality - (TargetQuality * (decimal)0.08);

                if (KpiQuality >= TargetQuality)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor1");
                }
                else if (KpiQuality >= deviation)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor2");
                }
                else if (KpiQuality > 0)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor3");
                }
                else
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor4");
                }

                return style;
            }
        }

        public Style ImageAvailability
        {
            get
            {
                Style style = ResourceHelper.GetResource<Style>("StatusPointColor1");


                decimal deviation = TargetAvailability - (TargetAvailability * (decimal)0.08);

                if (KpiAvailability >= TargetAvailability)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor1");
                }
                else if (KpiAvailability >= deviation)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor2");
                }
                else if (KpiAvailability > 0)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor3");
                }
                else
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor4");
                }

                return style;
            }
        }

        public Style ImageProductivity
        {
            get
            {
                Style style = ResourceHelper.GetResource<Style>("StatusPointColor1");


                decimal deviation = TargetProductivity - (TargetProductivity * (decimal)0.08);

                if (KpiProductivity >= TargetProductivity)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor1");
                }
                else if (KpiProductivity >= deviation)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor2");
                }
                else if (KpiProductivity > 0)
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor3");
                }
                else
                {
                    style = ResourceHelper.GetResource<Style>("StatusPointColor4");
                }

                return style;
            }
        }

        private decimal _targetOEE = 0;
        public decimal TargetOEE
        {
            set
            {
                _targetOEE = value;
                OnPropertyChanged("TargetOEE");
            }
            get
            {
                return _targetOEE;
            }
        }

        private decimal _targetProductivity = 0;
        public decimal TargetProductivity
        {
            set
            {
                _targetProductivity = value;
                OnPropertyChanged("TargetProductivity");
            }
            get
            {
                return _targetProductivity;
            }
        }

        private decimal _targetPerformance = 0;
        public decimal TargetPerformance
        {
            set
            {
                _targetPerformance = value;
                OnPropertyChanged("TargetPerformance");
            }
            get
            {
                return _targetPerformance;
            }
        }

        private decimal _targetQuality = 0;
        public decimal TargetQuality
        {
            set
            {
                _targetQuality = value;
                OnPropertyChanged("TargetQuality");
            }
            get
            {
                return _targetQuality;
            }
        }

        private decimal _targetAvailability = 0;
        public decimal TargetAvailability
        {
            set
            {
                _targetAvailability = value;
                OnPropertyChanged("TargetAvailability");
            }
            get
            {
                return _targetAvailability;
            }
        }


        public int IdDivision { set; get; }
        public string ActualCode { set; get; }

        public WorkCenter MyWorkCenter { set; get; }

        private int? _day = null;
        public int? Day
        {
            set
            {
                _day = value;
                OnPropertyChanged("Day");
            }
            get
            {
                return _day;
            }
        }

        private int? _week = null;
        public int? Week
        {
            set
            {
                _week = value;
                OnPropertyChanged("Week");
            }
            get
            {
                return _week;
            }
        }

        private int? _month = null;
        public int? Month
        {
            set
            {
                _month = value;
                OnPropertyChanged("Month");
            }
            get
            {
                return _month;
            }
        }

        private int? _year = null;
        public int? Year
        {
            set
            {
                _year = value;
                OnPropertyChanged("Year");
            }
            get
            {
                return _year;
            }
        }

        public string ShowByValue
        {
            get
            {
                string result = "";
             
                if (Month.HasValue)
                {
                    result = Year.ToString() + "/" + Month.ToString().PadLeft(2, '0');
                }
                else if (Week.HasValue)
                {
                    result = Year.ToString() + "/" + Week.ToString().PadLeft(2, '0');
                }
                else if (Year.HasValue)
                {
                    result = Year.ToString();
                }
                else if (Day.HasValue)
                {
                    result = Year.ToString() + "/" + Day.ToString().PadLeft(3, '0');
                }
                else if (Date.HasValue)
                {
                    result = Date.Value.ToShortDateString();
                }

                return result;
            }
        }

        private DateTime? _date = null;
        public DateTime? Date
        {
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
            get
            {
                return _date;
            }
        }

    }

    public class Calculation
    {
        public static double CalculateFunction(string functionKey, Dictionary<string, double> values)
        {
            double result = 0;

            var func = COSContext.Current.MathFunctions.FirstOrDefault(a => a.Key == functionKey);

            if (func != null)
            {
                dotMath.EqCompiler compiler = new dotMath.EqCompiler(true);

                compiler.SetFunction(func.Function);
                foreach (var itm in values)
                {
                    compiler.SetVariable(itm.Key, itm.Value);
                }

                result = compiler.Calculate();

                if (double.IsInfinity(result) || double.IsNaN(result))
                    result = 0;
            }

            return result;
        }
    }

    public enum ShowBy
    {
        All = 0,
        Day = 1,
        Week = 2,
        Month = 3,
        Year = 4
    }
}
