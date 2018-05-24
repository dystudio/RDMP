﻿using CatalogueLibrary.Repositories;
using RDMPAutomationService;
using RDMPAutomationService.Options;

namespace RDMPAutomationServiceTests.AutomationLoopTests
{
    internal class MockAutomationServiceOptions : RunOptions
    {
        private readonly IRDMPPlatformRepositoryServiceLocator mockRepositoryLocator;
        
        public MockAutomationServiceOptions(IRDMPPlatformRepositoryServiceLocator mockRepositoryLocator)
        {
            this.mockRepositoryLocator = mockRepositoryLocator;
        }

        public override IRDMPPlatformRepositoryServiceLocator GetRepositoryLocator()
        {
            return mockRepositoryLocator;
        }
    }
}