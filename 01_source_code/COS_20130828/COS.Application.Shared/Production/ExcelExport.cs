using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared.Production
{
    public class ExcelExport
    {

        public static void ExportProductions(IEnumerable<SelectProduction> productions, string fileName)
        {
            using (COS.Excel.COSExcel exc = new Excel.COSExcel())
            {
                int row = 1;

                var listprods = productions.ToList();
                foreach (var itm in listprods)
                {
                    //exc.SetData(1, row, 1, itm.Date);
                    //exc.SetData(1, row, 2, itm.Week);
                    //exc.SetData(1, row, 3, itm.Shift_Color);
                    //exc.SetData(1, row, 4, itm.);
                    //exc.SetData(1, row, 5, itm.Pracovní_skupina);
                    //exc.SetData(1, row, 6, itm.Pracovní_středisko);
                    //exc.SetData(1, row, 7, itm.Hodina);
                    //exc.SetData(1, row, 8, itm.Zakázka);
                    //exc.SetData(1, row, 9, itm.Položka);
                    //exc.SetData(1, row, 10, itm.Čas_k_dispozici);
                    //exc.SetData(1, row, 11, itm.Operátoři_Lindab);
                    //exc.SetData(1, row, 12, itm.Operátoři_Brig_);
                    //exc.SetData(1, row, 13, itm.Operátoři_celkem);
                    //exc.SetData(1, row, 14, itm.Vyrobené_kusy);
                    //exc.SetData(1, row, 15, itm.Špatné_kusy);
                    //exc.SetData(1, row, 16, itm.Váha_odpadu);
                    //exc.SetData(1, row, 17, itm.Vážený_odpad);
                    //exc.SetData(1, row, 18, itm.Poznámka);
                    //exc.SetData(1, row, 19, itm.Prostoj);
                    //exc.SetData(1, row, 20, itm.Prostoj___Produktivita);
                    //exc.SetData(1, row, 21, itm.Skut___Operační_čas);
                    //exc.SetData(1, row, 22, itm.Skut___Čas_ideální_takt);
                    //exc.SetData(1, row, 23, itm.Skut___Kusů_Hlava_Hodina);
                    //exc.SetData(1, row, 24, itm.Plán___Počet_operátorů);
                    //exc.SetData(1, row, 25, itm.Plán___Kusů_za_hodinu);
                    //exc.SetData(1, row, 26, itm.Plán___Kusů_Hlava_Hodina);
                    //exc.SetData(1, row, 27, itm.Produktivita);
                    //exc.SetData(1, row, 28, itm.Výkon);
                    //exc.SetData(1, row, 29, itm.Kvalita);
                    //exc.SetData(1, row, 30, itm.Dostupnost);

                    row++;
                }

                exc.Save(fileName);
            }
        }
    }
}
