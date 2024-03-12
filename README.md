# Quote of the Day

"Quote of the Day" is a .NET 8 application built with ASP.NET Razor pages. It shows a random quote when loaded. A user can heart-vote on the quote. The application is to demonstrate the configuration, feature management, and experimentation capabilities of **Azure App Configuration**.

![Screenshot of my app](images\screenshot.png)

## Configuration
The application loads quotes from App Configuration, so you can update the quote of the day without redeploying or restarting the application.

## Feature management & experimentation
Instead of the standard title, the application displays a greeting for certain users based on the targeting rules of a feature flag. The experimentation (aka. A/B testing) is conducted to understand if a greeting message can encourage more heart-voting from users.

## Telemetry
After running the application, loging in with different users, and voting for some quotes, You should find *FeatureEvaluation* and *HeartQuote* events as customEvents under the *Logs* balde in Application Insights.

## How to setup
1. Provision an App Configuration store and an Application Insights instance. Create following environment variables before you run the application.

    ```bash
    AZUREAPPCONFIGURATION_ENDPOINT="The endpoint of your Azure App Configuration"
    APPLICATIONINSIGHTS_CONNECTION_STRING="The connection string of your Application Insights"
    ```

1. Create a key-value to hold the quotes in your App Configuration store, for example:

    | Key | Value | Content-Type |
    | --- | ----- | ------------ |
    | QuoteOfTheDay:Quotes | <pre>[<br>  {<br>    "Quote": "Example quote",<br>    "Author": "Author name"<br>  }<br>]</pre> | application/json |

1. Create a feature flag named **Greeting** (key name *.appconfig.featureflag/QuoteOfTheDay:Greeting*) with two variants *On* and *Off*, and add allocation rules as you like.
