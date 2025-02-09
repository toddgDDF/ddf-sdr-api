﻿using System.Collections.Generic;

namespace TransCelerate.SDR.Core.DTO.Study
{
    /// <summary>
    /// This class is a DTO for GET Method for AuditTrail of a study
    /// </summary>
    public class GetStudyAuditDTO
    {    
        public string studyId { get; set; }     
        public List<AuditTrailEndpointResponseDTO> auditTrail { get; set; }
    }

    public class AuditTrailEndpointResponseDTO
    {
        public string studyTag { get; set; }
        public string studyStatus { get; set; }
        public string entryDateTime { get; set; }
        public string entrySystem { get; set; }
        public int studyVersion { get; set; }
    }
}
