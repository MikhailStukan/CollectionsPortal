﻿using CollectionsPortal.Models;

namespace CollectionsPortal.ViewModels
{
    public class CreateCollectionViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Topic Topic { get; set; }

        public List<FieldTemplate> Fields { get; set; } = new();
    }
}
