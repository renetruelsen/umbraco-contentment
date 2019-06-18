﻿/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class ByteSizeConfigurationEditor : ConfigurationEditor
    {
        public const string Decimals = "decimals";
        public const string Filter = "filter";
        public const string Format = "format";
        public const string Kilo = "kilo";

        public ByteSizeConfigurationEditor()
        {
            Fields.Add(
                Kilo,
                "Kilobytes?",
                "How many bytes are there in a Kilobyte?<br>Kilobyte (1000), kibibyte (1024), you decide.",
                IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { AllowEmptyConfigurationField.AllowEmpty, Constants.Values.False },
                    { DropdownListConfigurationEditor.Items, new[]
                        {
                            new DataListItem { Name = "1000 Bytes", Value = "1000" },
                            new DataListItem { Name = "1024 Bytes", Value = "1024" },
                        }
                    },
                    { DropdownListConfigurationEditor.DefaultValue, "1024" },
                });

            Fields.Add(
                Decimals,
                "Decimal places",
                "How many decimal places would you like?",
                IOHelper.ResolveUrl("~/umbraco/views/propertyeditors/slider/slider.html"),
                new Dictionary<string, object>
                {
                    { "initVal1", 2 },
                    { "minVal", 0 },
                    { "maxVal", 10 },
                    { "step", 1 }
                });
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.ContainsKey(Filter) == false)
            {
                // NOTE: Unfortunately this wont work with v8.0.2, as the ReadOnlyValueController is expecting an array object.
                // https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.2/src/Umbraco.Web.UI.Client/src/views/propertyeditors/readonlyvalue/readonlyvalue.controller.js#L18-L22
                // TODO: [LK:2019-06-07] Patch supplied to Umbraco: https://github.com/umbraco/Umbraco-CMS/pull/5615
                config.Add(Filter, "formatBytes");

                if (config.ContainsKey(Format) == false && config.ContainsKey(Kilo) && config.ContainsKey(Decimals))
                {
                    config.Add(Format, new
                    {
                        kilo = config[Kilo],
                        decimals = config[Decimals]
                    });

                    config.Remove(Kilo);
                    config.Remove(Decimals);
                }
            }

            return config;
        }
    }
}