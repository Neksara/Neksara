    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    namespace Neksara.ViewModels
    {
        // ViewModel untuk menampilkan/mengelola Student beserta Courses yang diambil
        public class TopicViewVM
        {

            public List<string> TopicNames { get; set; } = new();

            // untuk checkbox
            public List<int> SelectedTopicIds { get; set; } = new();
        }
        
    }