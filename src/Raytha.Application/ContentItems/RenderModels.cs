﻿using Raytha.Application.Common.Interfaces;
using Raytha.Application.Common.Models;
using Raytha.Application.Common.Models.RenderModels;
using Raytha.Application.ContentTypes;
using Raytha.Application.Views;
using Raytha.Domain.Entities;
using System.Linq.Expressions;
using System.Text;

namespace Raytha.Application.ContentItems;

public record ContentItem_RenderModel : IInsertTemplateVariable
{
    public string Id { get; init; }
    public AuditableUser_RenderModel? CreatorUser { get; init; }
    public AuditableUser_RenderModel? LastModifierUser { get; init; }
    public DateTime CreationTime { get; init; }
    public DateTime? LastModificationTime { get; init; }
    public string? Template { get; init; }
    public ContentType_RenderModel? ContentType { get; init; }
    public string PrimaryField { get; init; }
    public dynamic PublishedContent { get; init; }
    public string RoutePath { get; init; }

    public static Expression<Func<ContentItemDto, ContentItem_RenderModel>> GetProjection()
    {
        return entity => GetProjection(entity);
    }
    public static ContentItem_RenderModel GetProjection(ContentItemDto entity)
    {
        return new ContentItem_RenderModel
        {
            Id = entity.Id,
            CreatorUser = AuditableUser_RenderModel.GetProjection(entity.CreatorUser),
            CreationTime = entity.CreationTime,
            LastModifierUser = AuditableUser_RenderModel.GetProjection(entity.LastModifierUser),
            LastModificationTime = entity.LastModificationTime,
            Template = entity.WebTemplate?.DeveloperName,
            ContentType = ContentType_RenderModel.GetProjection(entity.ContentType),
            PrimaryField = entity.PrimaryField,
            PublishedContent = entity.PublishedContent,
            RoutePath = entity.RoutePath
        };
    }

    public override string ToString()
    {
        return PrimaryField;
    }

    public IEnumerable<string> GetDeveloperNames()
    {
        yield return BuiltInContentTypeField.Id.DeveloperName;
        yield return BuiltInContentTypeField.CreationTime.DeveloperName;
        yield return BuiltInContentTypeField.LastModificationTime.DeveloperName;
        yield return BuiltInContentTypeField.PrimaryField.DeveloperName;
        yield return BuiltInContentTypeField.Template.DeveloperName;
        yield return nameof(RoutePath);
        foreach (var developerName in AuditableUser_RenderModel.FromPrefix(nameof(LastModifierUser)).GetDeveloperNames())
        {
            yield return developerName;
        }
        foreach (var developerName in AuditableUser_RenderModel.FromPrefix(nameof(CreatorUser)).GetDeveloperNames())
        {
            yield return developerName;
        }
    }

    public IEnumerable<KeyValuePair<string, string>> GetTemplateVariables()
    {
        foreach (var developerName in GetDeveloperNames())
        {
            yield return new KeyValuePair<string, string>(developerName, $"Target.{developerName}");
        }
    }

    public string GetTemplateVariablesAsForEachLiquidSyntax()
    {
        StringBuilder sb = new StringBuilder(string.Empty);
        sb.AppendLine($"{{% for item in Target.Items %}}");
        foreach (var developerName in GetDeveloperNames())
        {
            sb.AppendLine($"{developerName}: item.{developerName}");
        }
        sb.AppendLine($"{{% endfor %}}");
        return sb.ToString();
    }
}

public record ContentItemListResult_RenderModel : IInsertTemplateVariable
{
    public IEnumerable<ContentItem_RenderModel> Items { get; set; }
    public string Search { get; set; } = string.Empty;
    public string Filter { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 0;
    public int PageSize { get; set; } = 0;
    public string OrderBy { get; set; } = string.Empty;
    public int TotalCount { get; set;  }

    public bool PreviousDisabledCss => TotalPages == 0 || PageNumber == 1;
    public bool NextDisabledCss => TotalPages == 0 || PageNumber == TotalPages;
    public int FirstVisiblePageNumber => Math.Max(1, LastVisiblePageNumber - 3);
    public int LastVisiblePageNumber => Math.Min(TotalPages, Math.Max(1, PageNumber - 1) + 3);
    public int TotalPages => (int)Math.Ceiling((double)(TotalCount) / PageSize);

    public string RoutePath { get; set; }
    public string DeveloperName { get; set; }
    public string Label { get; set; }

    public static ContentItemListResult_RenderModel GetProjection(
        ListResultDto<ContentItemDto> entity,
        ViewDto view,
        string search = "",
        string filter = "",
        string orderBy = "",
        int pageSize = 50,
        int pageNumber = 1)
    {
        var convertedToList = entity.Items.Select(p => ContentItem_RenderModel.GetProjection(p)).ToList();
        return new ContentItemListResult_RenderModel
        {
            Items = convertedToList,
            TotalCount = entity.TotalCount,
            Search = search,
            Filter = filter,
            OrderBy = orderBy,
            PageSize = pageSize,
            PageNumber = pageNumber < 1 ? 1 : pageNumber,
            RoutePath = view.RoutePath,
            DeveloperName = view.DeveloperName,
            Label = view.Label
        };
    }

    public IEnumerable<string> GetDeveloperNames()
    {
        yield return nameof(Items);
        yield return nameof(TotalCount);
        yield return nameof(Search);
        yield return nameof(Filter);
        yield return nameof(OrderBy);
        yield return nameof(PageSize);
        yield return nameof(PageNumber);

        yield return nameof(PreviousDisabledCss);
        yield return nameof(NextDisabledCss);
        yield return nameof(FirstVisiblePageNumber);
        yield return nameof(LastVisiblePageNumber);
        yield return nameof(TotalPages);

        yield return (nameof(RoutePath));
        yield return (nameof(DeveloperName));
        yield return (nameof(Label));
    }

    public IEnumerable<KeyValuePair<string, string>> GetTemplateVariables()
    {
        foreach (var developerName in GetDeveloperNames())
        {
            yield return new KeyValuePair<string, string>(developerName, $"Target.{developerName}");
        }
    }

    public string GetTemplateVariablesAsForEachLiquidSyntax()
    {
        StringBuilder sb = new StringBuilder(string.Empty);
        sb.AppendLine($"{{% for item in Target.Items %}}");
        foreach (var developerName in GetDeveloperNames())
        {
            sb.AppendLine($"{developerName}: {{ item.{developerName}.Text }}");
        }
        sb.AppendLine($"{{% endfor %}}");
        return sb.ToString();
    }
}
