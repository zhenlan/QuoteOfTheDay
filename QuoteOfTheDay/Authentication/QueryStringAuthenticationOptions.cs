// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//
using Microsoft.AspNetCore.Authentication;

namespace QuoteOfTheDay.Authentication
{
    class QueryStringAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string UsernameParameterName { get; set; } = "username";

        public string GroupsParameterName { get; set; } = "groups";
    }
}
