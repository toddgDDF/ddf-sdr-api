﻿using AutoMapper;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransCelerate.SDR.Core.DTO;
using TransCelerate.SDR.Core.DTO.Study;
using TransCelerate.SDR.Core.Entities.Study;
using TransCelerate.SDR.Core.Utilities;
using TransCelerate.SDR.Core.Utilities.Common;
using TransCelerate.SDR.Core.Utilities.Helpers;
using TransCelerate.SDR.DataAccess.Interfaces;
using TransCelerate.SDR.Services.Interfaces;

namespace TransCelerate.SDR.Services.Services
{
    public class ClinicalStudyService : IClinicalStudyService
    {
        #region Variables
        private readonly IClinicalStudyRepository _clinicalStudyRepository;
        private readonly IMapper _mapper;
        private readonly ILogHelper _logger;
        #endregion

        #region Constructor
        public ClinicalStudyService(IClinicalStudyRepository clinicalStudyRepository, IMapper mapper, ILogHelper logger)
        {
            _clinicalStudyRepository = clinicalStudyRepository;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        #region ActionMethods
        #region GET Methods

        /// <summary>
        /// GET All Elements For a Study
        /// </summary>
        /// <param name="studyId"></param>
        /// <param name="version"></param>
        /// <param name="tag"></param>
        /// <returns>
        /// A <see cref="GetStudyDTO"/> with matching studyId <br></br> <br></br>
        /// <see langword="null"/> If no study is matching with studyId
        /// </returns>
        public async Task<GetStudyDTO> GetAllElements(string studyId,int version, string tag)
        {
            try
            {
                _logger.LogInformation($"Started Service : {nameof(ClinicalStudyService)}; Method : {nameof(GetAllElements)};");
                studyId = studyId.Trim();
               
                StudyEntity study;
                if (String.IsNullOrWhiteSpace(tag))
                {
                    study = await _clinicalStudyRepository.GetStudyItemsAsync(studyId: studyId, version: version).ConfigureAwait(false);
                }
                else
                {
                    tag = tag.Trim();
                    study = await _clinicalStudyRepository.GetStudyItemsAsync(studyId: studyId, version: version, tag: tag).ConfigureAwait(false);
                }

                if (study == null)
                {                    
                    return null;
                }
                else
                {                       
                    var studyDTO = _mapper.Map<GetStudyDTO>(study);  //Mapping Entity to Dto                                                  
                    return studyDTO;                  
                }
            }
            catch (Exception)
            {                
                throw;
            }
            finally
            {
                _logger.LogInformation($"Ended Service : {nameof(ClinicalStudyService)}; Method : {nameof(GetAllElements)};");
            }
        }

        /// <summary>
        /// GET All Elements For a Study
        /// </summary>
        /// <param name="studyId"></param>
        /// <param name="version"></param>
        /// <param name="tag"></param>
        /// <param name="sections"></param>
        /// <returns>
        /// A <see cref="object"/> of study sections with matching studyId <br></br> <br></br>
        /// <see langword="null"/> If no study is matching with studyId
        /// </returns>
        public async Task<object> GetSections(string studyId, int version, string tag, string[] sections)
        {
            try
            {
                _logger.LogInformation($"Started Service : {nameof(ClinicalStudyService)}; Method : {nameof(GetSections)};");
                studyId = studyId.Trim();

                StudyEntity study;
                if (String.IsNullOrWhiteSpace(tag))
                {
                    study = await _clinicalStudyRepository.GetStudyItemsAsync(studyId: studyId, version: version).ConfigureAwait(false);
                }
                else
                {
                    tag = tag.Trim();
                    study = await _clinicalStudyRepository.GetStudyItemsAsync(studyId: studyId, version: version, tag: tag).ConfigureAwait(false);
                }

                if (study == null)
                {
                    return null;
                }
                else
                {                    
                    var studySectionDTO = _mapper.Map<GetStudySectionsDTO>(study.clinicalStudy);
                    studySectionDTO.studyVersion = study.auditTrail.studyVersion;                                                           
                   
                    return RemoveStudySections.RemoveSections(sections, studySectionDTO); //Remove the sections which are not part of sections array
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _logger.LogInformation($"Ended Service : {nameof(ClinicalStudyService)}; Method : {nameof(GetSections)};");
            }
        }

        /// <summary>
        /// GET For a StudyDesign sections for a study
        /// </summary>
        /// <param name="studyId"></param>
        /// <param name="version"></param>
        /// <param name="tag"></param>
        /// <param name="sections"></param>
        /// <param name="studyDesignId"></param>
        /// <returns>
        /// A <see cref="object"/> of studyDesign sections with matching studyId <br></br> <br></br>
        /// <see langword="null"/> If no study is matching with studyId
        /// </returns>
        public async Task<object> GetStudyDesignSections(string studyId, string studyDesignId, int version, string tag, string[] sections)
        {
            try
            {
                _logger.LogInformation($"Started Service : {nameof(ClinicalStudyService)}; Method : {nameof(GetStudyDesignSections)};");
                studyId = studyId.Trim();

                StudyEntity study;
                if (String.IsNullOrWhiteSpace(tag))
                {
                    study = await _clinicalStudyRepository.GetStudyItemsAsync(studyId: studyId, version: version).ConfigureAwait(false);
                }
                else
                {
                    tag = tag.Trim();
                    study = await _clinicalStudyRepository.GetStudyItemsAsync(studyId: studyId, version: version, tag: tag).ConfigureAwait(false);
                }

                if (study == null)
                {
                    return null;
                }
                else
                {                    
                    var studySectionDTO = _mapper.Map<GetStudySectionsDTO>(study.clinicalStudy); //Mapping Entity to Dto  
                    studySectionDTO.studyVersion = study.auditTrail.studyVersion;
                    studySectionDTO.studyDesigns = studySectionDTO.studyDesigns != null? studySectionDTO.studyDesigns.FindAll(x => x.studyDesignId == studyDesignId).Count()!=0 ? studySectionDTO.studyDesigns.FindAll(x => x.studyDesignId == studyDesignId).ToList(): new List<GetStudyDesignsDTO>() : new List<GetStudyDesignsDTO>();                    
                   
                    return RemoveStudySections.RemoveSectionsForStudyDesign(sections, studySectionDTO); //Remove the sections which are not part of sections array
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _logger.LogInformation($"Ended Service : {nameof(ClinicalStudyService)}; Method : {nameof(GetStudyDesignSections)};");
            }
        }

        /// <summary>
        /// GET Audit Trial
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="studyId"></param>
        /// <returns>
        /// A <see cref="GetStudyAuditDTO"/> with matching studyId <br></br> <br></br>
        /// <see langword="null"/> If no study is matching with studyId
        /// </returns>
        public async Task<GetStudyAuditDTO> GetAuditTrail(DateTime fromDate, DateTime toDate, string studyId)
        {
            try
            {
                _logger.LogInformation($"Started Service : {nameof(ClinicalStudyService)}; Method : {nameof(GetAuditTrail)};");                             
                var studies = await _clinicalStudyRepository.GetAuditTrail(fromDate, toDate, studyId);
                if(studies == null)
                {                    
                    return null;
                }
                else
                {
                    var auditTrailDTOList = _mapper.Map<List<AuditTrailEndpointResponseDTO>>(studies); //Mapping Entity to Dto 
                    GetStudyAuditDTO getStudyAuditDTO = new GetStudyAuditDTO
                    {
                        studyId = studyId, auditTrail = auditTrailDTOList
                    };
                    
                    return getStudyAuditDTO;
                }
            }
            catch (Exception)
            {                
                throw;
            }
            finally
            {
                _logger.LogInformation($"Ended Service : {nameof(ClinicalStudyService)}; Method : {nameof(GetAuditTrail)};");
            }
        }

        /// <summary>
        /// Get AllStudy Id's
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="studyTitle"></param>
        /// <returns>
        /// A <see cref="GetStudyHistoryResponseDTO"/> which has list of study ID's <br></br> <br></br>
        /// <see langword="null"/> If no study is matching with studyId
        /// </returns>
        public async Task<GetStudyHistoryResponseDTO> GetAllStudyId(DateTime fromDate, DateTime toDate, string studyTitle)
        {
            try
            {
                _logger.LogInformation($"Started Service : {nameof(ClinicalStudyService)}; Method : {nameof(GetAllStudyId)};");
                var studies = await _clinicalStudyRepository.GetAllStudyId(fromDate, toDate, studyTitle); //Getting List of studyId, studyTitle and Version
                if (studies == null)
                {
                    return null;
                }
                else
                {                    
                    var groupStudy = studies.GroupBy(x => new { x.studyId })
                                            .Select(g => new
                                            {
                                                studyId = g.Key.studyId,
                                                studyTitle = g.Select(x => x).Where(x => x.studyVersion == g.Max(x => x.studyVersion)).FirstOrDefault().studyTitle,
                                                studyVersion = g.Select(x => x.studyVersion).OrderBy(x => x).ToArray(),
                                                date = g.Select(x => x).Where(x => x.studyVersion == g.Max(x => x.studyVersion)).FirstOrDefault().entryDateTime
                                            }) // Grouping the Id's by studyId
                                            .OrderByDescending(x => x.date)
                                            .ToList();

                    GetStudyHistoryResponseDTO allStudyIdResponseDTO = new GetStudyHistoryResponseDTO() //Mapping Group to Study History ResponseDto 
                    {
                        study = JsonConvert.DeserializeObject<List<StudyHistoryDTO>>(JsonConvert.SerializeObject(groupStudy))
                    };

                    return allStudyIdResponseDTO;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _logger.LogInformation($"Ended Service : {nameof(ClinicalStudyService)}; Method : {nameof(GetAllStudyId)};");
            }
        }
        #endregion


        #region POST Methods
        /// <summary>
        /// POST All Elements For a Study
        /// </summary>
        /// <param name="studyDTO"></param>
        /// <param name="entrySystem"></param>
        /// <param name="entrySystemId"></param>
        /// <returns>
        /// A <see cref="PostStudyResponseDTO"/> which has study ID and study design ID's <br></br> <br></br>
        /// <see langword="null"/> If the insert is not done
        /// </returns>
        public async Task<object> PostAllElements(PostStudyDTO studyDTO,string entrySystem)
        {
            try
            {
                _logger.LogInformation($"Started Service : {nameof(ClinicalStudyService)}; Method : {nameof(PostAllElements)};");                  
                var incomingstudyEntity = _mapper.Map<StudyEntity>(studyDTO);           //Mapping Dto to Entity                
                #region Adding Audit Trail for Incoming Study
                AuditTrailEntity auditTrailEntity = new AuditTrailEntity();
                incomingstudyEntity.auditTrail = auditTrailEntity;
                incomingstudyEntity.auditTrail.entryDateTime = DateTime.UtcNow;
                incomingstudyEntity.auditTrail.entrySystem = entrySystem; 
                #endregion

                PostStudyResponseDTO postStudyDTO = new PostStudyResponseDTO(); //This class is for sending response for the POST Request

                if (String.IsNullOrEmpty(incomingstudyEntity.clinicalStudy.studyId)) 
                {
                    incomingstudyEntity.auditTrail.studyVersion = 1; 
                    incomingstudyEntity.clinicalStudy.studyId = IdGenerator.GenerateId(); 
                    incomingstudyEntity._id = ObjectId.GenerateNewId(); 
                    incomingstudyEntity.clinicalStudy.studyIdentifiers.ForEach(x => x.studyIdentifierId = IdGenerator.GenerateId()); //UUID for studyIdentifiers
                    SectionIdGenerator.GenerateSectionId(incomingstudyEntity); //UUID for all sections

                    #region Previous and Next Items Logic
                    PreviousItemNextItemHelper.PreviousItemsNextItemsWraper(incomingstudyEntity);
                    #endregion

                    _logger.LogInformation($"entrySystem: {entrySystem??"<null>"}; Study Input : {JsonConvert.SerializeObject(incomingstudyEntity)}");
                    postStudyDTO.studyId =  await _clinicalStudyRepository.PostStudyItemsAsync(incomingstudyEntity).ConfigureAwait(false);
                    
                    #region Response ID mapping
                    postStudyDTO.studyVersion = incomingstudyEntity.auditTrail.studyVersion;
                    var studyDesign = incomingstudyEntity.clinicalStudy.currentSections != null ? incomingstudyEntity.clinicalStudy.currentSections.FindAll(x => x.studyDesigns != null).ToList() : new List<CurrentSectionsEntity>();
                    if (studyDesign.Count() != 0)
                    {
                        var designIdList = new List<string>();
                        foreach (var item in studyDesign.Find(x => x.studyDesigns != null).studyDesigns)
                        {
                            designIdList.Add(item.studyDesignId);
                        }
                        postStudyDTO.studyDesignId = designIdList;
                    } 
                    #endregion
                }
                else //If there is a studyId in the input
                {
                    StudyEntity existingStudyEntity = _clinicalStudyRepository.GetStudyItemsAsync(incomingstudyEntity.clinicalStudy.studyId, 0).Result;                  
                    
                    if(existingStudyEntity == null)  
                    {
                        return Constants.ErrorMessages.NotValidStudyId;
                    }
                    else
                    {
                        existingStudyEntity.auditTrail.entryDateTime = incomingstudyEntity.auditTrail.entryDateTime;
                        existingStudyEntity.auditTrail.entrySystem = incomingstudyEntity.auditTrail.entrySystem;                        
                        incomingstudyEntity._id = existingStudyEntity._id;
                        incomingstudyEntity.auditTrail.studyVersion = existingStudyEntity.auditTrail.studyVersion;

                        var duplicateExistingStudy = JsonConvert.DeserializeObject<StudyEntity>(JsonConvert.SerializeObject(existingStudyEntity)); // Creating duplicates for existing entity
                        var duplicateIncomingStudy = JsonConvert.DeserializeObject<StudyEntity>(JsonConvert.SerializeObject(incomingstudyEntity)); // Creating duplicates for incoming entity

                        if (PostStudyElementsCheck.StudyComparison(duplicateIncomingStudy, duplicateExistingStudy)) //If the data in existing and incoming are same
                        {
                            _logger.LogInformation($"Study Input : {JsonConvert.SerializeObject(existingStudyEntity)}");
                            postStudyDTO.studyId = await _clinicalStudyRepository.UpdateStudyItemsAsync(existingStudyEntity); //update the existing latest version
                            postStudyDTO.studyVersion = existingStudyEntity.auditTrail.studyVersion;
                        }
                        else
                        {
                            incomingstudyEntity._id = ObjectId.GenerateNewId();
                            existingStudyEntity.auditTrail.studyVersion += 1;                            
                            PostStudyElementsCheck.SectionCheck(incomingstudyEntity, existingStudyEntity);

                            _logger.LogInformation($"Study Input : {JsonConvert.SerializeObject(existingStudyEntity)}");
                            existingStudyEntity._id = ObjectId.GenerateNewId();
                            postStudyDTO.studyId = await _clinicalStudyRepository.PostStudyItemsAsync(existingStudyEntity).ConfigureAwait(false);
                            postStudyDTO.studyVersion = existingStudyEntity.auditTrail.studyVersion;
                        }

                        #region Response ID mapping
                        var studyDesign = existingStudyEntity.clinicalStudy.currentSections != null ? existingStudyEntity.clinicalStudy.currentSections.FindAll(x => x.studyDesigns != null).ToList() : new List<CurrentSectionsEntity>();
                        if (studyDesign.Count() != 0)
                        {
                            var designIdList = new List<string>();
                            foreach (var item in studyDesign.Find(x => x.studyDesigns != null).studyDesigns)
                            {
                                designIdList.Add(item.studyDesignId);
                            }
                            postStudyDTO.studyDesignId = designIdList;
                        }
                        #endregion
                    }
                }
                return postStudyDTO;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _logger.LogInformation($"Ended Service : {nameof(ClinicalStudyService)}; Method : {nameof(PostAllElements)};");
            }
        }

        /// <summary>
        /// Search Study Elements with search criteria
        /// </summary>
        /// <param name="searchParameters"></param>
        /// <returns>
        /// A <see cref="List{GetStudyDTO}}"/> which matches serach criteria <br></br> <br></br>
        /// <see langword="null"/> If the insert is not done
        /// </returns>
        public async Task<List<GetStudyDTO>> SearchStudy(SearchParametersDTO searchParametersDTO)
        {
            try
            {
                _logger.LogInformation($"Started Service : {nameof(ClinicalStudyService)}; Method : {nameof(SearchStudy)};");
                _logger.LogInformation($"Search Parameters : {JsonConvert.SerializeObject(searchParametersDTO)}");

                var searchParameters = _mapper.Map<SearchParameters>(searchParametersDTO);

                var studies = await _clinicalStudyRepository.SearchStudy(searchParameters).ConfigureAwait(false);
                
                if (studies == null)
                {                  
                    return null;
                }
                else
                {
                    var studiesDTO = _mapper.Map<List<GetStudyDTO>>(studies); //Mapper to map from Entity to Dto
                  
                    return studiesDTO;
                }
            }
            catch (Exception)
            {                
                throw;
            }
            finally
            {
                _logger.LogInformation($"Ended Service : {nameof(ClinicalStudyService)}; Method : {nameof(SearchStudy)};");
            }
        }
        #endregion
        #endregion

    }
}
