# Digital Delta Eco specification

This specification describes the Digital Delta Eco API. It is meant for both implementers and consumers of the Digital Delta Eco specifications.

## About the Digital Delta Eco specification

This specification defines a standardised method for retrieving (and providing) [ecological measurements](https://github.com/DigitaleDeltaOrg/dd-eco-api-specs/wiki/Ecological-Measurements).

## Getting Started

The specification is written in [RAML 1.0](https://raml.org), a 'language' to define [REST](https://nl.wikipedia.org/wiki/Representational_state_transfer) (REpresentational State Transfer) service.
It is a specification, that means, there is no full implementation of this specification available in this repository.
There are, however, [result samples](https://github.com/DigitaleDeltaOrg/dd-eco-api-specs/tree/master/AquaDesk%20Examples) and a .NET sample implementation of the [Filter Syntax](https://github.com/DigitaleDeltaOrg/dd-eco-api-specs/wiki/Implementation-Topics).

To read the RAML specification, some [tools](https://github.com/DigitaleDeltaOrg/dd-eco-api-specs/wiki/Tools) are recommended. Some tools are also capable of generating documentation, or code skeletons for both consumers and implementers.

The [HTML specifications](https://github.com/DigitaleDeltaOrg/dd-eco-api-specs/tree/master/Html) can also be used and may be easier to read.

For testing, some [PostMan](https://getpostman.com) [scripts](https://github.com/DigitaleDeltaOrg/dd-eco-api-specs/tree/master/Postman%20Scripts) are supplied.
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

The easiest way to get the specification is to download the generated HTML files.
To be able to use the RAML files, please consult the [tools](https://github.com/DigitaleDeltaOrg/dd-eco-api-specs/wiki/Tools) section. Knowledge of the [RAML language](https://raml.org) is then recommended.

### Installing

The supplied code for the [.NET Filter Syntax Parser](https://github.com/DigitaleDeltaOrg/dd-eco-api-specs/tree/master/.NET%20Filter%20Syntax%20Parser) is written in C# with .NET Framework 4.6+.
A good environment for editing and compiling that solution, is [Visual Studio 2017 Community Edition](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=15).

The unit tests are written in [NUnit](http://nunit.org/).

## Authors

* **Geri Wolters** - *Initial work* - [EcoSys](https://www.ecosys.nl)
* **Stef Hummel** - *Initial work* - [Deltares](https://www.deltares.nl)

See also the list of [contributors](https://github.com/DigitaleDeltaOrg/dd-eco-api-specs/blob/master/Contributors.md) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](license.md) file for details

## Acknowledgments
