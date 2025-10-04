using ATIS.Views.Report;

namespace ATIS.Views.Main.Disease.DiseasePdf
{
    public class TblDiseasesPdfModel
    {
        public int DiseaseId
        {
            get; set;
        }
        public string DiseaseName
        {
            get; set;
        }
        public int CountId
        {
            get; set;
        }
        public bool? Valid
        {
            get; set;
        }
        public string ValidYear
        {
            get; set;
        }
        public string ShortInfo
        {
            get; set;
        }
        public string Symptom
        {
            get; set;
        }
        public string Reason
        {
            get; set;
        }
        public string Curability
        {
            get; set;
        }
        public string Therapie
        {
            get; set;
        }
        public string Info
        {
            get; set;
        }
        public string Writer
        {
            get; set;
        }
        public DateTime WriterDate
        {
            get; set;
        }
        public string Updater
        {
            get; set;
        }
        public DateTime UpdaterDate
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

        public PdfModels.Disease DiseaseTree
        {
            get;
            set;
        }
    }
}
