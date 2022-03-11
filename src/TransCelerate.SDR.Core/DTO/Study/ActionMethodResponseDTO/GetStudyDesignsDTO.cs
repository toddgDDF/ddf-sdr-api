﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TransCelerate.SDR.Core.DTO.Study
{
    public class GetStudyDesignsDTO
    {
        public string studyDesignId { get; set; }
        public string trialIntentType { get; set; }
        public string trialType { get; set; }

        public List<PlannedWorkflowDTO> plannedWorkflows { get; set; }

        public List<StudyPopulationDTO> studyPopulations { get; set; }

        public List<StudyCellDTO> studyCells { get; set; }

        public List<InvestigationalInterventionDTO> investigationalInterventions { get; set; }
    }
}
