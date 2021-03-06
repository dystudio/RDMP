--Version:1.30.0.0
--Description: Allows you to flag a specific ExtractionInformation as a PivotCategory which is used by the DQE and maybe in future the Aggregate engines to subdivide answers by a Catalogue wide category e.g. healthboard / country etc.

if not exists ( select 1 from sys.columns where name = 'PivotCategory_ExtractionInformation_ID')
begin
alter table Catalogue add PivotCategory_ExtractionInformation_ID [int] NULL
end

if not exists (select 1 from sys.foreign_keys where name = 'FK_PivotCategory_ExtractionInformation_ID') 
begin
ALTER TABLE [dbo].Catalogue  WITH CHECK ADD  CONSTRAINT FK_PivotCategory_ExtractionInformation_ID FOREIGN KEY(PivotCategory_ExtractionInformation_ID)
REFERENCES [dbo].ExtractionInformation ([ID])
end

