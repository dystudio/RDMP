--Version:1.48.0.1
--Description: Removes the ability to have multiple ExtractionInformation objects associated with a single CatalogueItem, this isn't possible through UI but can be done currently through API
 if not exists (select 1 from sys.indexes where name = 'ix_preventMultipleExtractionInformationsPerCatalogueItem')
 begin

 CREATE UNIQUE NONCLUSTERED INDEX [ix_preventMultipleExtractionInformationsPerCatalogueItem] ON [dbo].[ExtractionInformation]
(
	[CatalogueItem_ID] ASC
)
 end