using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared.TM
{
    public class Helper
    {
        public static void ProcessTool(ProcessType processType, HourlyProduction production)
        {
            //najdem si spojení s itemnumber
            var toolin = COSContext.Current.ToolItemNumbers.FirstOrDefault(a => a.ItemNumber == production.ItemNumber);

            if (toolin != null)
            {
                //když spojení existuje tak si vrátíme konkrétní tool
                var tool = toolin.Tool;
                if (tool != null)
                {
                    //vyhledáme spojení pro workcenter
                    var toolWC = tool.ToolWCs.FirstOrDefault(a => a.ID_workCenter == production.WorkCenter.ID);

                    //když ještě neexistuje, tak ho založíme
                    if (toolWC == null)
                    {
                        toolWC = COSContext.Current.TmToolWCs.CreateObject();
                        toolWC.Tool = tool;
                        toolWC.WorkCenter = production.WorkCenter;
                        COSContext.Current.TmToolWCs.AddObject(toolWC);
                    }

                    //dále přidáme nebo ubere ActualPcs podle operace kterou provádíme
                    if (processType == ProcessType.Insert)
                    {
                        ProcessInsert(production, toolWC);
                    }
                    else if (processType == ProcessType.Update)
                    {
                        ProcessUpdate(production, toolWC);
                    }
                    else if (processType == ProcessType.Delete)
                    {
                        ProcessDelete(production, toolWC);
                    }                    
                }
            }
        }

        private static void ProcessInsert(HourlyProduction production, TmToolWC toolWC)
        {
            toolWC.ActualPcs += production.ProducedPieces;
        }

        private static void ProcessUpdate(HourlyProduction production, TmToolWC toolWC)
        {
            var prodEntry = COSContext.Current.ObjectStateManager.GetObjectStateEntry(production);
            object origVal = prodEntry.OriginalValues["ProducedPieces"];
            object curVal = prodEntry.CurrentValues["ProducedPieces"];
            if (origVal != null && curVal != null)
                toolWC.ActualPcs += (decimal)curVal - (decimal)origVal;
        }

        private static void ProcessDelete(HourlyProduction production, TmToolWC toolWC)
        {
            toolWC.ActualPcs -= production.ProducedPieces;
        }


        public enum ProcessType
        {
            Insert = 1,
            Update = 2,
            Delete = 3
        }
    }
}
