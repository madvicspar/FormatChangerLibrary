using FormatChanger.Models.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models.FormattingModels
{
    public class ICaptionSettingsModel
    {
        public long Id { get; set; }

        [ForeignKey("TextSettings")]
        public long TextSettingsId { get; set; }

        /// <summary>
        /// Шаблон текста для подписей, например «Таблица N»
        /// </summary>
        public string TextTemplate { get; set; }

        /// <summary>
        /// Разделитель между номером и содержанием подписи, например « — »
        /// </summary>
        public string Separator { get; set; }

        public CaptionPosition Position { get; set; }

        public virtual TextSettingsModel TextSettings { get; set; }
    }

    public class ImageCaptionSettingsModel : ICaptionSettingsModel { }
    public class TableCaptionSettingsModel : ICaptionSettingsModel { }
}
