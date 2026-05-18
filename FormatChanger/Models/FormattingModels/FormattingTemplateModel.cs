using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using FormatChanger.Models;

namespace FormatChanger.Models.FormattingModels
{
    public class FormattingTemplateModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        [ForeignKey("TextSettings")]
        public long TextSettingsId { get; set; }
        [ForeignKey("HeadingSettings")]
        public long HeadingSettingsId { get; set; }
        [ForeignKey("TableSettings")]
        public long TableSettingsId { get; set; }
        [ForeignKey("ListSettings")]
        public long ListSettingsId { get; set; }
        [ForeignKey("ImageSettings")]
        public long ImageSettingsId { get; set; }
        [ForeignKey("DocumentSettings")]
        public long DocumentSettingsId { get; set; }
        [ValidateNever] public virtual TextSettingsModel TextSettings { get; set; }
        [ValidateNever] public virtual HeadingSettingsModel HeadingSettings { get; set; }
        [ValidateNever] public virtual TableSettingsModel TableSettings { get; set; }
        [ValidateNever] public virtual ListSettingsModel ListSettings { get; set; }
        [ValidateNever] public virtual ImageSettingsModel ImageSettings { get; set; }
        [ValidateNever] public virtual DocumentSettingsModel DocumentSettings { get; set; }
		[NotMapped]
		public List<HeadingLevelEditItem> HeadingLevelsEdit { get; set; } = [];

		public FormattingTemplateModel()
        {
            TextSettings = new TextSettingsModel();
            HeadingSettings = new HeadingSettingsModel();
            TableSettings = new TableSettingsModel();
            ListSettings = new ListSettingsModel();
            ImageSettings = new ImageSettingsModel();
            DocumentSettings = new DocumentSettingsModel();
        }
    }
}
