﻿/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.Models.ContentEditing;
using UmbracoIcons = Umbraco.Core.Constants.Icons;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class UmbracoEntityDataListSource : IDataListSource
    {
        internal static Dictionary<string, UmbracoObjectTypes> SupportedEntityTypes = new Dictionary<string, UmbracoObjectTypes>
        {
            { nameof(UmbracoEntityTypes.DataType), UmbracoObjectTypes.DataType },
            { nameof(UmbracoEntityTypes.Document), UmbracoObjectTypes.DocumentType },
            { nameof(UmbracoEntityTypes.DocumentType), UmbracoObjectTypes.DocumentType },
            { nameof(UmbracoEntityTypes.Media), UmbracoObjectTypes.Media },
            { nameof(UmbracoEntityTypes.MediaType), UmbracoObjectTypes.MediaType },
            { nameof(UmbracoEntityTypes.Member), UmbracoObjectTypes.Member },
            { nameof(UmbracoEntityTypes.MemberType), UmbracoObjectTypes.MemberType },
        };

        internal static Dictionary<string, string> EntityTypeIcons = new Dictionary<string, string>
        {
            { nameof(UmbracoEntityTypes.DataType), UmbracoIcons.DataType },
            { nameof(UmbracoEntityTypes.DocumentType), "icon-item-arrangement" },
            { nameof(UmbracoEntityTypes.MediaType), "icon-thumbnails" },
            { nameof(UmbracoEntityTypes.Member), UmbracoIcons.Member },
            { nameof(UmbracoEntityTypes.MemberType), UmbracoIcons.MemberType },
        };

        private readonly IEntityService _entityService;

        public UmbracoEntityDataListSource()
            : this(Current.Services.EntityService)
        {
            // TODO: Can we pass in the IEntityService? How to do DI with JSON.NET deserialization? [LK]
        }

        public UmbracoEntityDataListSource(IEntityService entityService)
        {
            _entityService = entityService;
        }

        public string Name => "Umbraco Entity";

        public string Description => "Select an Umbraco entity type to populate the data source.";

        public string Icon => "icon-science";

        [ConfigurationField(typeof(EntityTypeConfigurationField))]
        public string EntityType { get; set; }

        public IEnumerable<DataListItemModel> GetItems()
        {
            if (SupportedEntityTypes.TryGetValue(EntityType, out var objectType))
            {
                var icon = EntityTypeIcons.ContainsKey(EntityType) ? EntityTypeIcons[EntityType] : this.Icon;

                return _entityService
                    .GetAll(objectType)
                    .OrderBy(x => x.Name)
                    .Select(x => new DataListItemModel
                    {
                        Icon = icon,
                        Name = x.Name,
                        Value = new GuidUdi(EntityType, x.Key).ToString(),
                    });
            }

            return null;
        }

        class EntityTypeConfigurationField : ConfigurationField
        {
            public EntityTypeConfigurationField()
            {
                var items = SupportedEntityTypes.Keys.Select(x => new DataListItemModel { Name = x.SplitPascalCasing(), Value = x });

                Key = "entityType";
                Name = "Entity Type";
                Description = "Select the entity type to use.<br><br>Unsupported entity types have been hidden from the list.";
                View = IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>()
                {
                    { Constants.Conventions.ConfigurationEditors.Items, items }
                };
            }
        }
    }
}