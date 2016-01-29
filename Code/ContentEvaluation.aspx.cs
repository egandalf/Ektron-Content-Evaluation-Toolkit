using Ektron.Cms;
using Ektron.Cms.BusinessObjects.ContentWorkflow;
using Ektron.Cms.Content;
using Ektron.Cms.Framework.Content;
using Ektron.Cms.Framework.Organization;
using Ektron.Cms.Framework.Settings;
using Ektron.Cms.Settings;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class handlers_devhelp_ContentEvaluation : System.Web.UI.Page
{
    private SmartFormConfigurationManager SmartFormCRUD = new SmartFormConfigurationManager(Ektron.Cms.Framework.ApiAccessMode.Admin);

    private double _MinutesPerItem;
    private double _SecondsPerField;
    private double _PercentQA;
    private double _MinutesPerQA;
    private double _ManualEffort;
    private double _VelocityThreshold;

    private double _accumulatedHours = 0;
    private double _daysInYear = 252; // Yes, there are 365 days in a year, but most content changes happen M-F. This number removes weekends.

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            bool valueSuccess = false;
            double tmpValue = 0;
            valueSuccess = double.TryParse(uxMinutesPerItem.Text, out tmpValue);
            if (valueSuccess)
            {
                this._MinutesPerItem = tmpValue;

                valueSuccess = double.TryParse(uxSecondsPerField.Text, out tmpValue);
                if (valueSuccess)
                {
                    this._SecondsPerField = tmpValue;

                    valueSuccess = double.TryParse(uxPercentQA.Text, out tmpValue);
                    if (valueSuccess)
                    {
                        this._PercentQA = tmpValue;

                        valueSuccess = double.TryParse(uxQAMinutes.Text, out tmpValue);
                        if (valueSuccess)
                        {
                            this._MinutesPerQA = tmpValue;
                        }

                        valueSuccess = double.TryParse(uxInternalTime.Text, out tmpValue);
                        if (valueSuccess)
                        {
                            this._ManualEffort = tmpValue;

                            string velSetting = uxVelocityThreshold.SelectedValue;
                            if (velSetting == "monthly")
                                this._VelocityThreshold = 12d / _daysInYear;
                            else if (velSetting == "bimonthly")
                                this._VelocityThreshold = 24d / _daysInYear;
                            else if (velSetting == "weekly")
                                this._VelocityThreshold = 52d / _daysInYear;
                            else if (velSetting == "biweekly")
                                this._VelocityThreshold = 104d / _daysInYear;
                            else if (velSetting == "daily")
                                this._VelocityThreshold = 1d;
                        }
                    }
                }
            }
            if (!valueSuccess)
            {
                throw new ApplicationException("One of the input fields contained an invalid value.");
            }
        }
    }

    protected void uxGetResults_Click(object sender, EventArgs e)
    {
        //uxDataCollection.Visible = false;
        instructions.Attributes["class"] = "invisible";
        toggleInstructionsBtn.InnerText = "Show Instructions";

        List<ContentTypeInformation> ContentTypeSummary = this.GetContentTypeInformation();
        this.PopulateContentTypeTable(ContentTypeSummary);

        Dictionary<long, double> ValueEstimates = this.CalculateValue(ContentTypeSummary);

        var valueData = ValueEstimates.Select(v => new
        {
            Name = ContentTypeSummary.First(c => c.ID == v.Key).Name,
            Value = Math.Round(v.Value / 60, 2),
            Class = ClassForItem(Math.Round(v.Value / 60, 2), ContentTypeSummary.First(c => c.ID == v.Key).Velocity),
            Message = ClassForItem(Math.Round(v.Value / 60, 2), ContentTypeSummary.First(c => c.ID == v.Key).Velocity) == "manual" ? "Consider for manual migration." : "Consider for scripted/tooled migration."
        }).OrderByDescending(v => v.Value);

        uxValues.DataSource = valueData;
        uxValues.DataBind();
    }

    private string ClassForItem(double manHours, decimal velocity)
    {
        if (velocity < (decimal)this._VelocityThreshold)
        {
            _accumulatedHours += manHours;
            if (_accumulatedHours < _ManualEffort)
            {
                return "manual";
            }
        }
        return "script";
    }

    private Dictionary<long, double> CalculateValue(List<ContentTypeInformation> ContentTypeSummary)
    {
        Dictionary<long, double> AggregateValues = new Dictionary<long, double>();

        double baseMinutes = this._MinutesPerItem;
        double minutesPerField = this._SecondsPerField / 60d;
        double baseQAMinutes = this._MinutesPerQA;
        double percentForQA = this._PercentQA;
        double hoursForManual = this._ManualEffort;

        double baseMinutesForType = 0;
        double minutesPerFieldForType = 0;
        double itemsForQA;
        double totalMinutes;

        foreach (var t in ContentTypeSummary)
        {
            // base + field
            minutesPerFieldForType = minutesPerField * (double)t.FieldCount;
            baseMinutesForType = baseMinutes + minutesPerFieldForType;
            totalMinutes = baseMinutesForType * t.Volume;

            // qa
            itemsForQA = (double)t.FieldCount * percentForQA / 100d;
            
            if (t.RequiresApproval == ContentTypeInformation.Approval.Yes)
                itemsForQA *= 2d;
            else if (t.RequiresApproval == ContentTypeInformation.Approval.Sometimes)
                itemsForQA *= 1.5d;

            totalMinutes += itemsForQA * baseQAMinutes;

            AggregateValues.Add(t.ID, totalMinutes);
        }

        return AggregateValues;
    }

    private void PopulateContentTypeTable(List<ContentTypeInformation> ContentTypeSummary)
    {
        uxContentData.DataSource = ContentTypeSummary;
        uxContentData.DataBind();
    }

    private List<ContentTypeInformation> GetContentTypeInformation()
    {
        var typeList = new List<ContentTypeInformation>();
        var criteria = GetCriteria();
        var results = SmartFormCRUD.GetList(criteria);
        if (results.Any())
        {
            typeList = results.Select(s => new ContentTypeInformation(s)).ToList();
            typeList.Add(new ContentTypeInformation(new SmartFormConfigurationData() { 
                Id = 0, 
                SmartformTitle = "HTML Content",
                FieldList = "<fieldlist><field name=\"HTML\" datatype=\"string\" basetype=\"text\" xpath=\"/\" title=\"HTML\">HTML</field></fieldlist>"
            }));
        }
        return typeList.OrderBy(f => f.Name).ToList();
    }

    private SmartFormConfigurationCriteria GetCriteria()
    {
        var criteria = new SmartFormConfigurationCriteria(Ektron.Cms.Common.SmartFormConfigurationProperty.Title, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        criteria.PagingInfo = new PagingInfo(9999);
        return criteria;
    }
}

public class ContentTypeInformation
{
    private ContentManager ContentCRUD = new ContentManager(Ektron.Cms.Framework.ApiAccessMode.Admin);
    private FolderManager FolderCRUD = new FolderManager(Ektron.Cms.Framework.ApiAccessMode.Admin);

    public enum Approval
    {
        Yes,
        No,
        Sometimes
    }

    public string Name { get; set; }
    public long ID { get; set; }
    public int FieldCount { get; set; }
    public string Fields { get; set; }
    public int Volume { get; set; }
    public decimal Velocity { get; set; }
    public string Folders { get; set; }
    public Approval RequiresApproval { get; set; }

    public ContentTypeInformation(SmartFormConfigurationData ContentTypeData)
    {
        this.Name = ContentTypeData.SmartformTitle;
        this.ID = ContentTypeData.Id;

        var typeFields = GetFields(ContentTypeData.FieldList);

        this.Fields = ConvertFieldsToHtml(typeFields);

        this.FieldCount = typeFields.Count();
        this.Volume = GetTypeVolume(ContentTypeData.Id);

        if (this.Volume > 0)
        {
            List<long> folderIds = null;

            decimal tmpVelocity = CalculateVelocity(ContentTypeData.Id, out folderIds);

            List<FolderData> typeFolders = GetFolders(folderIds);
            var folderArray = typeFolders.Select(f => f.NameWithPath).ToArray();
            this.Folders = ConvertFieldsToHtml(folderArray);

            this.Velocity = Math.Round(tmpVelocity, 4);

            this.RequiresApproval = CheckRequiresApproval(typeFolders);
        }
        else
        {
            this.Velocity = 0;
            this.RequiresApproval = Approval.No;
        }
    }

    private List<FolderData> GetFolders(List<long> folderIds)
    {
        List<FolderData> folderList = new List<FolderData>();
        if (folderIds.Any())
        {
            FolderCriteria criteria = GetFolderCriteria(folderIds);
            folderList = FolderCRUD.GetList(criteria);
            if (criteria.PagingInfo.TotalPages > 1)
            {
                for (int i = 2; i <= criteria.PagingInfo.TotalPages; i++)
                {
                    criteria.PagingInfo.CurrentPage = i;
                    folderList.AddRange(FolderCRUD.GetList(criteria));
                }
            }
        }
        return folderList;
    }

    private FolderCriteria GetFolderCriteria(List<long> folderIds)
    {
        var criteria = new FolderCriteria(Ektron.Cms.Common.FolderProperty.FolderPath, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
        criteria.AddFilter(Ektron.Cms.Common.FolderProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.In, folderIds);
        criteria.PagingInfo = new PagingInfo(99, 1);
        return criteria;
    }

    private Approval CheckRequiresApproval(List<FolderData> folders)
    {
        if (folders.Any())
        {
            List<Approval> storedApprovals = new List<Approval>();

            // Couldn't find way to work around legacy APIs. If Approval calculation is desired, this must be run within the site.
            var cwUtilities = new ContentWorkflowUtilities(CommonApi.Current.RequestInformationRef);
            EkContent _content = new EkContent(CommonApi.Current.RequestInformationRef);

            int chainSize = 0;
            long wfid = 0;
            Collection items = null;

            foreach (FolderData f in folders)
            {
                chainSize = 0;
                wfid = 0;

                items = _content.GetApprovalInfov2_0(f.Id, "folder");
                chainSize = int.Parse(items[1].ToString());

                wfid = cwUtilities.GetInheritedWorkflowDefinitionId(0, f.Id, CommonApi.Current.RequestInformationRef.ContentLanguage);

                if (chainSize > 0 || wfid > 0)
                {
                    if (!storedApprovals.Contains(Approval.Yes))
                        storedApprovals.Add(Approval.Yes);
                }
                else
                {
                    if (!storedApprovals.Contains(Approval.No))
                        storedApprovals.Add(Approval.No);
                }

                if (storedApprovals.Contains(Approval.Yes) &&
                    storedApprovals.Contains(Approval.No))
                    break;
            }

            if (storedApprovals.Contains(Approval.Yes) &&
                storedApprovals.Contains(Approval.No))
                return Approval.Sometimes;
            else if (storedApprovals.Contains(Approval.Yes))
                return Approval.Yes;
            else
                return Approval.No;
        }
        return Approval.No;
    }

    private decimal CalculateVelocity(long ContentTypeID, out List<long> folders)
    {
        ContentData latestItem = GetLatestContent(ContentTypeID);

        var lastDateModifed = latestItem.DateModified;
        var yearPrior = lastDateModifed.AddYears(-1);

        var criteria = GetContentCriteria(ContentTypeID, yearPrior, lastDateModifed);
        criteria.PagingInfo = new PagingInfo(999, 1);

        var contentList = new List<ContentData>();
        contentList = ContentCRUD.GetList(criteria);
        if (criteria.PagingInfo.TotalPages > 1)
        {
            for (int i = 2; i <= criteria.PagingInfo.TotalPages; i++)
            {
                criteria.PagingInfo.CurrentPage = i;
                contentList.AddRange(ContentCRUD.GetList(criteria));
            }
        }

        var count = 0;
        folders = new List<long>();
        foreach (var item in contentList)
        {
            // collect folder for later processing
            if (!folders.Contains(item.FolderId))
                folders.Add(item.FolderId);

            var H = ContentCRUD.GetHistoryList(item.Id);
            if (H == null)
            {
                count++;
            }
            else
            {
                var history = H.Where(h => h.Status == "A");
                foreach (var version in history)
                {
                    if (DateTime.Compare(yearPrior, version.DateInserted) <= 0 &&
                        DateTime.Compare(lastDateModifed, version.DateInserted) >= 0)
                        count++;
                }
            }
        }

        return (decimal)count / (decimal)252; // Yes, there are 365 days in a year, but most content changes happen M-F. This number removes weekends.
    }

    private ContentData GetLatestContent(long ContentTypeID)
    {
        var criteria = GetContentCriteria(ContentTypeID);
        var listOfOne = ContentCRUD.GetList(criteria);
        if (listOfOne.Any())
        {
            return listOfOne.First();
        }
        return null;
    }

    private int GetTypeVolume(long ContentTypeID)
    {
        var criteria = GetContentCriteria(ContentTypeID);
        var listOfOne = ContentCRUD.GetList(criteria);
        return criteria.PagingInfo.TotalPages;
    }

    private ContentCriteria GetContentCriteria(long ContentTypeID, DateTime? StartDate = null, DateTime? EndDate = null)
    {
        var criteria = new ContentCriteria(Ektron.Cms.Common.ContentProperty.DateModified, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Descending);
        criteria.AddFilter(Ektron.Cms.Common.ContentProperty.XmlConfigurationId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, ContentTypeID);
        if (StartDate.HasValue && EndDate.HasValue)
        {
            criteria.AddFilter(Ektron.Cms.Common.ContentProperty.DateModified, Ektron.Cms.Common.CriteriaFilterOperator.GreaterThanOrEqualTo, StartDate.Value);
            criteria.AddFilter(Ektron.Cms.Common.ContentProperty.DateModified, Ektron.Cms.Common.CriteriaFilterOperator.LessThanOrEqualTo, EndDate.Value);
        }
        criteria.PagingInfo = new PagingInfo(1, 1);
        return criteria;
    }

    private string ConvertFieldsToHtml(string[] fields)
    {
        //var formattedStrings = fields.Select(f => string.Format("<div>{0}</div>", f)).ToArray();
        return string.Join("\r\n<span/>", fields);
    }

    private string[] GetFields(string fieldXML)
    {
        var xdoc = XDocument.Parse(fieldXML);
        var fields = xdoc.Document.Element("fieldlist").Elements("field");
        var paths = fields.Attributes("xpath");
        return paths.Select(p => p.Value).ToArray();
    }
}