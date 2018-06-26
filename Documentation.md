# Introduction
<a name="Introduction"></a>
This document is a proposal for an extension to the Digital Delta API, specifically for parameter-based queries for mainly [ecological measurements](#EcologicalMeasurements), as opposed to the time-series focused approach that the original DD API used.

To make filtering easy, the [Filter Syntax](#FilterSyntax) is developed.
A sample implementation in C# for the Filter Syntax, including NUnit tests, is provided in this repository. See [Contents](#Contents).

The document describes the required parts ([Measurements level](#Measurements)) and optional (discovery) parts ([Discovery level](#Discovery)).

Minimal only contains the definitions needed to retrieve measurements. The user will need to know the exact codes or names of the items to query.

Extended provides definitions to discover information, such as finding the codes for parameters, find the measurement objects belonging to a monitoring network, etc. It is build on top of the [Measurements level](#Measurements) specification.
Every end-point in the [Discovery level](#Discovery) specification is optional.

Providers (providers of data via this API) can use the definition to implement their own connector for this specification to allow their customers to be consumers of their data.
Consumers (requesters of data via this API) can use the definition to connect to providers.
The samples given are based on the AquaDesk implementation of this specification.
The [tools](#Tools) section provides information concerning suggested tools for easier using of and building solutions for this specification.

To help testing, the [Contents](#Contents) section also provides sample [PostMan](https://www/getpostman.com) Collection 2.1 scripts for sample queries.
To help reading the specification, the [Contents](#Contents) section also provides an HTML directory, which contains pre-rendered HTML versions of this specification.

# Ecological Measurements
<a name="EcologicalMeasurements"></a>
Ecological measurements are divers, and mostly not a goal, but a means. A means to produce a spreadsheet, display a map,
analyse what happens to a species in time, analyse the quality of a body of water, required reporting to the government, etc.

To be able to provide relevant information, the following is stored with a measurement:

| Topic | Description |
| --- | -- |
| Who | What organisation requested the measurement and/or performed the sampling/analysis. |
| When | When the sample was taken, the analysis performed. Ecological measurements are mostly non-automated and non-continuous. |
| Where | Where the measurement was taken. This includes a position, but also what compartment such as open water, shore, vegetation. Some measurements are taken on a boat, so the position of the measurement may not be fixed and can differ from the location where the measurement object. |
| What | This can range from a single chemical, to multiple kinds of species per measurement. Next to the parameter (mostly applies to species), more specifications might be requested, such as life stage, gender, behaviour. |
| Why | The reason why the measurement took place. Was it, for instance, for a regular monitoring, was it for a specific project, was it for assessments, etc. |
| How | The how described how a sample was taken, for instance: what fish net was used, what size the holes in the net were, what handbook was used to sample, or to analyse, etc. |

To allow those questions to be answered, the information required to answer those questions, need to be stored. That makes ecological measurements more complex, since that meta-information varies.

Furthermore, many ecological measurements require manual labour (getting the sample, preparing the sample, observing the sample) and are not measured in a fixed interval.
Time series are therefore a much less useful format for this type of measurements.  

# Tools
<a name="Tools"></a>
RAML is not an easy specification to read, let alone write. However, there are several tools that help. The RAML site itself keeps a list of [available tools](https://raml.org/projects). Be careful though, as not all of these tools support RAML 1.0.
The suggested tools are all free or open source. For some tools a paid version with more functionality is also available.

Some suggestions:

## Editors
<a name="Editors"></a>

| Editor | Comment |
| --- | --- |
| [api-designer](https://github.com/mulesoft/api-designer) | Browser-based. No search features. |
| [atom](https://atom.io/) editor with [api-workbench](https://atom.io/packages/api-workbench) package and (optionally) the [markdown-preview package](https://atom.io/packages/markdown-preview) | Full-featured editor, but sometimes painfully slow with large files. |
For more suggestions see: [RAML Developers Design page](https://raml.org/developers/design-your-api).

## Documentation generators
<a name="DocumentationGenerators"></a>

| Documentation generator | Comment |
| --- | --- |
| [raml2html](https://www.npmjs.com/package/raml2html) | Generates static single-page HTML from a RAML definition. |
| [api-console](https://github.com/mulesoft/api-console) | Generates dynamic single=page HTML from a RAML definition. Generates HUGE files. Can also serve the files. Allows dynamic requests to be send to a server. |  

For more suggestions see: [RAML Developers Documentation page](https://raml.org/developers/document-your-api).

## Code generators
<a name="CodeGenerators"></a>

For more suggestions see: [RAML Developers Build page](https://raml.org/developers/build-your-api).

## Testing
<a name="Testing"></a>

| Test tool | Comment |
| --- | --- |
| [PostMan](https://www.getpostman.com) | PostMan is a REST testing tool. |
| [SoapUI](https://www.soapui.org/open-source.html) | SoapUI is a REST and SOAP testing tool. |  

For more suggestions see: [RAML Developers Test page](https://raml.org/developers/test-your-api).

# Two levels of the API
<a name="TwoLevels"></a>
The API is split into two levels:

## Measurements Level
<a name="Measurements"></a>
The Measurements level contains one end-point: Measurements.
This end-point is used to retrieve measurements.
No discovery end-points exist in this level: it is assumed that all knowledge concerning query parameters is known by the requester.
This level is required.

## Discovery Level
<a name="Discovery"></a>
The Discovery level is build on top of the Measurements level. Its purpose is to allow discovery of query parameters.
All end-points are optional, as is the whole level.

# Implementation topics
<a name="Implementation"></a>
## Filter Syntax
<a name="FilterSyntax"></a>
Filtering can be performed by constructing a search filter using the following syntax:

&lt;parametername&gt;:&lt;comparer&gt;:&lt;comparevalue&gt;[;&lt;parametername&gt;:&lt;comparer&gt;:&lt;comparevalue&gt;]<br/>

| Comparer | Description | Data types |
|----------|-------------|------|
| eq | equal | string, date, number |
| ne | unequal | string, date, number |
| lt | less than | date, number |
| le | less than or equal to | date, number |
| ge | greater than or equal to | date, number |
| gt | greater than | date, number |
| in | item is one of the values in the list | string, number |
| not | item is NOT one of the values in the list | string, number |
| like | String contains the value. | string |
| startswith | String starts with the value. | string |
| endswith | String ends with the value. | string |
| inbbox | Item is within the bounding box, defined by 4 numbers | bbox |
| inpolygon | Item is within the polygon, defined by list of numbers, where each x and y are a numbers in the array | bbox |
| notinbbox | Item is NOT within the bounding box, defined by 4 numbers | polygon |
| notinpolygon | Item is NOT within the polygon, defined by list of numbers, where each x and y are a numbers in the array | polygon |
| wkt | Item is within the Well-known-text definition | Wkt |
| all | all items in the list must be present in the queried item | string, number |

| Formatting |
| --- |
| String compares are NOT case sensitive and must be surrounded by quotes. |
| Arrays are expressed in JSON format. This means that they are surrounded by square brackets ([ and ]) and the array values are separated by comma's. |
| Parameter names are NOT case sensitive. |
| Number formats are in US notation. This means fractional separators are periods. Comma's (thousands-separators) are ignored. |
| Date field notation is yyyy-MM-dd and must be surrounded by quotes. |
| Bbox type requires an array of 4 numeric values. The uneven-numbers represent an X-ordinate, the even-numbers represent an Y-ordinates. The uneven and the following even number represent a coordinate in WGS84. |
| Polygon type requires an array of numeric values. The number of items in the array need to be even. The uneven-numbers represent an X-ordinate, the even-numbers represent an Y-ordinates. The uneven and the following even number represent a coordinate in WGS84. |

| Examples |
| --- |
| location&colon;eq&colon;"NLKAD";parameter&colon;eq&colon;"Eukariota";organisation&colon;eq&colon;"RWS",measuredvalue&colon;gt&colon;1000;measuredunitSymbol&colon;eq&colon;"n" |
| location&colon;in&colon;["NKLAD","NKLBVA","NLKBRA"];parameter&colon;eq&colon;"Eukariota";organisation&colon;eq&colon;"RWS";measuredvalue&colon;gt&colon;1000,measuredunitSymbol&colon;eq&colon;"n" |
| location&colon;in&colon;["NKLAD","NKLBVA","NLKBRA"];parameter&colon;in&colon;["Eukariota [1]","Plantae"];measuredvalue&colon;gt&colon;1000;measuredunitSymbol&colon;eq&colon;"n" |
| locationArea&colon;inpolygon&colon;[1,2,3,4];parameterCode&colon;in&colon;["Eukariota [1]","Plantae"];measuredvalue&colon;gt&colon;1000; |

Example of a filter in a URL:
/organisations/?filter=organisation:eq:"RWS"

Sorting is the responsibility of the caller. It is the responsibility of the provider to always return results in the same order.

General URL syntax:
available parameters on the root level:

| Field |  Meaning |
| --- |  --- |
| filter | See ['FilterSyntax'](#FilterSyntax). |
| page | Returns the specified page of records. See ['Paging logic'](#PagingLogic).  |
| pagesize | The maximum number of records returned for the page. See ['Paging logic'](#PagingLogic). |

## Error Handling
<a name="ErrorHandling"></a>
When errors are encountered, error information is provided as follows:
The HTTP status code is set to be a value between 400 and 499.
The response body has the structure of the Problem type (see 'Minimal'):

```
{
  "type": "Filter query syntax",
  "title": "Query syntax",
  "typeinfo": null,
  "status": 400,
  "detail": "See 'errors' attribute.",
  "ìnstance": null,
  "errors": [
      {
          "errortype": "InvalidValue",
          "context": "symbol:eq:"
      }
  ]
}
  ```

The value of field 'type' contains an error message.
The value of field 'typeinfo' is optional and can contain a link to a page explaining the message.
The value of field 'title' contains the title of the message.
The value of field 'status' has contains the HTTP status code.
The value of field 'detail' can provide extra information, such as given in the example.
The value of field 'instance' is optional and can contain information that identifies the error and request at the provider.
The field 'errors' is optional and can contain further information concerning the error. In the example, it explains what is wrong with the query syntax.

## Paging logic
<a name="PagingLogic"></a>
### Initialisation
<a name="PagingLogicInitialisation"></a>
The minimum page&gt; is 1.
The provider specifies minimum, maximum and default pagesize&gt;.
If pagesize&gt; is not specified, pagesize&gt; is the provider-specified default page size.
If page&gt; is not specified, page&gt; is assumed to be 1.

### Paging process
<a name="PagingLogicPagingProcess"></a>
* If &lt;filter&gt; is set,  &lt;filter&gt; is applied to the data set.
* ((&lt;page&gt; - 1) * &lt;pagesize&gt;) records are skipped.
* A maximum of &lt;pagesize&gt; records are returned from the resulting data set.

To find out what filters are recognised by the service, the /filters segment can be used on the endpoints. The filters segment does not recognise any additional parameters.

Also see ['Error handling'](#ErrorHandling) and ['Filter Syntax'](#FilterSyntax).

# Contents of the Repository
<a name="Contents"></a>
The structure of this repository is as follows:

| Directory | Purpose |
| --- | --- |
| &lt;root&gt; | Contains the base RAML definitions, the readme file and the change history. |
| .NET Filter Syntax Parser | Contains a sample implementation of a .NET project that parses the Filter Syntax including NUnit tests. |
| AquaDesk Examples | JSON responses from AquaDesk for demonstration purposes. |
| Html | Contains generated HTML documentation in two themes: the slate theme includes types.  |
| Libraries | RAML libraries in use. |
| PostMan Scripts | Contains sample PostMan scripts in PostMan Collection 2.1 format. |
| Security | Contains Security-related RAML definitions. |

# Change history (Specification)
<a name="ChangeHistory"></a>

| In Version | Change Date | Who | Description |
| -------- | ----------- | --- | ----------- |
| 0.9 | 20160621 | [Geri Wolters](mailto:geri.wolters@ecosys.nl) ([EcoSys](https://www.ecosys.nl)) | Initial release. |
