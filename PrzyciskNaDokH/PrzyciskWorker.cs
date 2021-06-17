using PrzyciskNaDokH;
using Soneta.Business;
using Soneta.Business.UI;
using Soneta.CRM;
using Soneta.Handel;
using Soneta.Zadania;
using System;
using System.Linq;

[assembly: Worker(typeof(PrzyciskWorker), typeof(DokumentHandlowy))]

namespace PrzyciskNaDokH
{
    public class PrzyciskWorker
    {

        Log log = new Log("Otworz ostatnie zadanie CRM worker", true);

        private void l(string t)
        {
#if DEBUG
            log.WriteLine(t);
#endif
        }

        [Context]
        public Context Context { get; set; }

        [Context]
        public DokumentHandlowy dokumentHandlowy { get; set; }

        [Action("Pokaż ostatnie zadanie CRM",
            Mode = ActionMode.SingleSession | ActionMode.Progress | ActionMode.OnlyForm,
            Priority = 1,
            Target = ActionTarget.ToolbarWithText,
            Icon = ActionIcon.ArrowRight)]
        public Zadanie OtworzOstatnieZadanieCRM()
        {


            l($"ping {DateTime.Now}");

            if (dokumentHandlowy == null)
            {
                l("dokument handlowy == null");
                return null;
            }


            Kontrahent kontrahent= dokumentHandlowy.Kontrahent;


            if (kontrahent == null)
            {
                l("kontrahent == null");
                return null;
            }

            Session session = this.Context.Session;
            Soneta.Zadania.ZadaniaModule zm = Soneta.Zadania.ZadaniaModule.GetInstance(session);

            Soneta.Zadania.Zadanie zadanie = zm.Zadania.WgKontrahent[kontrahent].ToList<Soneta.Zadania.Zadanie>().OrderByDescending(x => x.DataDo).ThenByDescending(x => x.CzasOd).FirstOrDefault();

            if (zadanie == null)
            {
                l($"brak zadań dla kontrahenta {kontrahent}");
                return null;
            }

            l($"zadanie: {zadanie.Numer}");

            return zadanie;

        }
    }


}
