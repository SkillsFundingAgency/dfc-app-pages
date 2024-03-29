﻿// <copyright file="AppSettings.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using TestAutomation.UI.Settings;

namespace DFC.App.Pages.Model
{
    internal class AppSettings : IAppSettings
    {
        public Uri AppBaseUrl { get; set; }
    }
}