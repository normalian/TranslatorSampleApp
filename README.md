# Azure Cognitive document translation sample
This sample offers to get started easily to utilize Azure Cognitive document translation service. Refer to an article below for the detail.
https://docs.microsoft.com/en-us/azure/cognitive-services/translator/document-translation/get-started-with-document-translation?tabs=csharp

## How to start 
you need to create Azure resources below.
- Azure Storage account and two containers at least
- Azure Cognitive Service

## How to configure
- Assign SAS for both source and target containers on your Azure Storage account. Pick up these SAS.
  - source container must have designated read and list access
  - target container must have designated write and list access
- Pick up "endpoint" of "Document Translation" and "subscription key" from Azure Cognitive Service. 

## Copyright
<table>
  <tr>
    <td>Copyright</td><td>Copyright (c) 2021 - Daichi Isami</td>
  </tr>
  <tr>
    <td>License</td><td>-</td>
  </tr>
</table>
