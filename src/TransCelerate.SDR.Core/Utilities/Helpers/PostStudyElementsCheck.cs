﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransCelerate.SDR.Core.Entities.Study;

namespace TransCelerate.SDR.Core.Utilities.Helpers
{
    public static class PostStudyElementsCheck
    {
        #region Full Study Check
        /// <summary>
        /// This method is used to check whether the incoming study and existing study are same
        /// </summary>
        /// <param name="incoming"></param>
        /// <param name="existing"></param>
        /// <returns>        
        /// <see langword="true"/> If incoming and existing study entities are identical
        /// </returns>
        public static bool StudyComparison(StudyEntity incoming, StudyEntity existing)
        {
            try
            {                          
                RemoveId(incoming);
                RemoveId(existing);

                return JsonObjectCheck(incoming, existing);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// This method will check whether two Json objects are same or not
        /// </summary>
        /// <param name="incoming"></param>
        /// <param name="existing"></param>
        /// <returns>        
        /// <see langword="true"/> If incoming and existing study entities are identical
        /// </returns>
        public static bool JsonObjectCheck(object incoming, object existing)
        {
            try
            {
                return JToken.DeepEquals(JObject.Parse(JsonConvert.SerializeObject(incoming)),
                               JObject.Parse(JsonConvert.SerializeObject(existing)));
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// This method will remove all the id fields in the study
        /// </summary>
        /// <param name="studyEntity"></param>
        /// <returns>
        /// A <see cref="StudyEntity"/> after removing all ID fields        
        /// </returns>
        public static StudyEntity RemoveId(StudyEntity studyEntity)
        {
            try
            {
                studyEntity.clinicalStudy.studyIdentifiers.ForEach(x => x.studyIdentifierId = null);
                if (studyEntity.clinicalStudy.currentSections != null)
                {
                    //Id Generation for section level
                    studyEntity.clinicalStudy.currentSections.ForEach(x => x.currentSectionsId = null);

                    if (studyEntity.clinicalStudy.currentSections.Any(x => x.studyDesigns != null))
                    {
                        studyEntity.clinicalStudy.currentSections
                                            .FindAll(x => x.studyDesigns != null)
                                            .ForEach(y => y.studyDesigns
                                            .FindAll(n => n.currentSections != null)
                                            .ForEach(z => z.currentSections
                                            .ForEach(i => i.currentSectionsId = null)));
                    }

                    //Remove Id for each section
                                           
                    //ObjectiveId
                    studyEntity.clinicalStudy.currentSections
                                            .FindAll(x => x.objectives != null)
                                            .ForEach(x => x.objectives
                                            .ForEach(y => {
                                                y.objectiveId = null;
                                                if (y.endpoints != null)
                                                {
                                                    y.endpoints.FindAll(x => x != null)
                                                           .ForEach(x => x.endPointsId = null);
                                                }
                                            }));

                    //studyIndicationId
                    studyEntity.clinicalStudy.currentSections
                                            .FindAll(x => x.studyIndications != null)
                                            .ForEach(x => x.studyIndications
                                            .ForEach(y => y.studyIndicationId = null));

                    //Removed StudyProtocol Section
                    //protocolId
                    //if (studyEntity.clinicalStudy.currentSections.Any(x => x.studyProtocol != null))
                    //{
                    //    studyEntity.clinicalStudy.currentSections.Find(x => x.studyProtocol != null)
                    //                                         .studyProtocol.protocolId = null;
                    //}

                    //studyDesignId
                    studyEntity.clinicalStudy.currentSections
                                            .FindAll(x => x.studyDesigns != null)
                                            .ForEach(x => x.studyDesigns
                                            .ForEach(y => y.studyDesignId = null));

                    //studyPopulationId
                    studyEntity.clinicalStudy.currentSections
                                            .FindAll(x => x.studyDesigns != null)
                                            .ForEach(y => y.studyDesigns
                                            .FindAll(n => n.currentSections != null)
                                            .ForEach(z => z.currentSections
                                            .FindAll(n => n.studyPopulations != null)
                                            .ForEach(p => p.studyPopulations
                                            .ForEach(i => i.studyPopulationId = null))));

                    //investigationalInterventionId
                    studyEntity.clinicalStudy.currentSections
                                            .FindAll(x => x.studyDesigns != null)
                                            .ForEach(y => y.studyDesigns
                                            .FindAll(n => n.currentSections != null)
                                            .ForEach(z => z.currentSections
                                            .FindAll(x => x.investigationalInterventions != null)
                                            .ForEach(x => x.investigationalInterventions
                                            .ForEach(y => y.investigationalInterventionId = null))));

                    //plannedWorkFlowId and sub-elements Id's
                    studyEntity.clinicalStudy.currentSections
                                            .FindAll(x => x.studyDesigns != null)
                                            .ForEach(y => y.studyDesigns
                                            .FindAll(n => n.currentSections != null)
                                            .ForEach(z => z.currentSections
                                            .FindAll(n => n.plannedWorkflows != null)
                                            .ForEach(p => p.plannedWorkflows
                                            .ForEach(i => {
                                                i.plannedWorkFlowId = null;
                                                if (i.startPoint != null)
                                                    i.startPoint.pointInTimeId = null;

                                                if (i.endPoint != null)
                                                    i.endPoint.pointInTimeId = null;

                                                if (i.workflowItemMatrix != null)
                                                {
                                                    i.workflowItemMatrix.workFlowItemMatrixId = null;
                                                    if (i.workflowItemMatrix.matrix!=null)
                                                    {                                                       
                                                        i.workflowItemMatrix.matrix.FindAll(x => x != null).ForEach(m =>
                                                        {
                                                            m.matrixId = null;
                                                            if(m.items!=null)
                                                            {
                                                                m.items.FindAll(x => x != null).ForEach(item =>
                                                                {
                                                                    item.itemId = null;
                                                                    if (item.fromPointInTime != null)
                                                                        item.fromPointInTime.pointInTimeId = null;

                                                                    if (item.toPointInTime != null)
                                                                        item.toPointInTime.pointInTimeId = null;

                                                                    if (item.activity != null)
                                                                    {
                                                                        item.activity.activityId = null;
                                                                        if(item.activity.studyDataCollection != null)
                                                                            item.activity.studyDataCollection.FindAll(x => x != null).ForEach(sdc => sdc.studyDataCollectionId = null); 
                                                                    }
                                                                    if (item.encounter != null)
                                                                    {
                                                                        item.encounter.encounterId = null;
                                                                        if (item.encounter.startRule != null)
                                                                            item.encounter.startRule.RuleId = null;

                                                                        if (item.encounter.endRule != null)
                                                                            item.encounter.endRule.RuleId = null;

                                                                        if (item.encounter.epoch != null)
                                                                            item.encounter.epoch.epochId = null;
                                                                    }
                                                                });
                                                            }
                                                        });
                                                    }
                                                }
                                            }))));

                    //studyCellId and sub-elements Id's
                    studyEntity.clinicalStudy.currentSections
                                            .FindAll(x => x.studyDesigns != null)
                                            .ForEach(y => y.studyDesigns
                                            .FindAll(n => n.currentSections != null)
                                            .ForEach(z => z.currentSections
                                            .FindAll(n => n.studyCells != null)
                                            .ForEach(p => p.studyCells
                                            .ForEach(i => {
                                                i.studyCellId = null;
                                                if (i.studyArm != null)
                                                    i.studyArm.studyArmId = null;

                                                if (i.studyEpoch != null)
                                                    i.studyEpoch.studyEpochId = null;

                                                if(i.studyElements!=null)
                                                {
                                                    i.studyElements.FindAll(x => x != null).ForEach(e => {
                                                        e.studyElementId = null;
                                                        e.startRule.RuleId = null;
                                                        e.endRule.RuleId = null;
                                                    });
                                                }
                                            }))));
                }
                return studyEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Study SectionCheck
        /// <summary>
        /// Section Level Study comparison for sectional POST
        /// </summary>
        /// <param name="incoming"></param>
        /// <param name="existing"></param>
        /// <returns>
        /// A <see cref="StudyEntity"/> after checking the section for Posting the data to the database       
        /// </returns>
        public static StudyEntity SectionCheck(StudyEntity incoming, StudyEntity existing)
        {
            try
            {
                existing.clinicalStudy.studyType = incoming.clinicalStudy.studyType;
                existing.clinicalStudy.studyTitle = incoming.clinicalStudy.studyTitle;
                existing.clinicalStudy.studyPhase = incoming.clinicalStudy.studyPhase ?? existing.clinicalStudy.studyPhase;                
                existing.clinicalStudy.studyTag = incoming.clinicalStudy.studyTag ?? existing.clinicalStudy.studyTag;
                existing.clinicalStudy.studyStatus = incoming.clinicalStudy.studyStatus ?? existing.clinicalStudy.studyStatus;

                //For studyIdentifiers
                var studyIdentifiersList = new List<StudyIdentifierEntity>();
                foreach(var studyIdentifier in incoming.clinicalStudy.studyIdentifiers)
                {
                    if(existing.clinicalStudy.studyIdentifiers.Any(x=>x.studyIdentifierId==studyIdentifier.studyIdentifierId))
                    {
                        studyIdentifiersList.Add(studyIdentifier);
                        existing.clinicalStudy.studyIdentifiers.RemoveAll(x => x.studyIdentifierId == studyIdentifier.studyIdentifierId);
                    }
                    else
                    {
                        studyIdentifier.studyIdentifierId = IdGenerator.GenerateId();
                        studyIdentifiersList.Add(studyIdentifier);
                    }                    
                }
                existing.clinicalStudy.studyIdentifiers = studyIdentifiersList;

                //If there is no current sections in existing entity, map the whole incoming current section with new one
                if (existing.clinicalStudy.currentSections == null && incoming.clinicalStudy.currentSections != null)
                {
                    existing.clinicalStudy.currentSections = SectionIdGenerator.GenerateSectionId(incoming).clinicalStudy.currentSections;
                }
                //If there is current sections in existing entity, the we are proceeding with section level checks
                else if (incoming.clinicalStudy.currentSections != null && existing.clinicalStudy.currentSections != null)
                {
                    List<InvestigationalInterventionEntity> existingInvestigationalInterventionEntities = existing.clinicalStudy.currentSections.FindAll(x => x.investigationalInterventions != null).Count() != 0 ? existing.clinicalStudy.currentSections.Find(x => x.investigationalInterventions != null).investigationalInterventions : new List<InvestigationalInterventionEntity>();
                    List<InvestigationalInterventionEntity> incomingInvestigationalInterventionEntities = incoming.clinicalStudy.currentSections.FindAll(x => x.investigationalInterventions != null).Count()!=0 ? incoming.clinicalStudy.currentSections.Find(x => x.investigationalInterventions != null).investigationalInterventions:new List<InvestigationalInterventionEntity>();

                    List<StudyObjectiveEntity> existingStudyObjectivesEntities = existing.clinicalStudy.currentSections.FindAll(x => x.objectives != null).Count()!=0 ? existing.clinicalStudy.currentSections.Find(x => x.objectives != null).objectives: new List<StudyObjectiveEntity>();
                    List<StudyObjectiveEntity> incomingStudyObjectivesEntities = incoming.clinicalStudy.currentSections.FindAll(x => x.objectives != null).Count() !=0 ? incoming.clinicalStudy.currentSections.Find(x => x.objectives != null).objectives: new List<StudyObjectiveEntity>();

                    List<StudyIndicationEntity> existingStudyIndicationEntities = existing.clinicalStudy.currentSections.FindAll(x => x.studyIndications != null).Count()!=0? existing.clinicalStudy.currentSections.Find(x => x.studyIndications != null).studyIndications:new List<StudyIndicationEntity>();
                    List<StudyIndicationEntity> incomingStudyIndicationEntities = incoming.clinicalStudy.currentSections.FindAll(x => x.studyIndications != null).Count() != 0 ? incoming.clinicalStudy.currentSections.Find(x => x.studyIndications != null).studyIndications : new List<StudyIndicationEntity>();                   

                    List<StudyDesignEntity> existingStudyDesignEntities = existing.clinicalStudy.currentSections.FindAll(x => x.studyDesigns != null).Count()!=0? existing.clinicalStudy.currentSections.Find(x => x.studyDesigns != null).studyDesigns: new List<StudyDesignEntity>();
                    List<StudyDesignEntity> incomingStudyDesignEntities = incoming.clinicalStudy.currentSections.FindAll(x => x.studyDesigns != null).Count() != 0 ? incoming.clinicalStudy.currentSections.Find(x => x.studyDesigns != null).studyDesigns : new List<StudyDesignEntity>();                                      

                    Parallel.Invoke(
                                //Study Objectives Section
                                ()=>StudyObjectivesSectionCheck(existing, existingStudyObjectivesEntities, incomingStudyObjectivesEntities),
                                //Study Indications Section
                                () => StudyIndicationSectionCheck(existing, existingStudyIndicationEntities, incomingStudyIndicationEntities),
                                //Study Design Section
                                () => StudyDesignSectionCheck(existing, existingStudyDesignEntities, incomingStudyDesignEntities)
                            );
                }                
                return existing;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Study Level Sections       
        /// <summary>
        /// Check for Study Objectives section 
        /// </summary>
        /// <param name="existing"></param>
        /// <param name="existingStudyObjectivesEntities"></param>
        /// <param name="incomingStudyObjectivesEntities"></param>
        public static void StudyObjectivesSectionCheck(StudyEntity existing, List<StudyObjectiveEntity> existingStudyObjectivesEntities, List<StudyObjectiveEntity> incomingStudyObjectivesEntities)
        {
            try
            {
                if (existingStudyObjectivesEntities.Count() == 0 && incomingStudyObjectivesEntities.Count() != 0)
                {
                    var newStudyObjectiveList = new List<StudyObjectiveEntity>();
                    foreach (var item in incomingStudyObjectivesEntities)
                    {
                        var newStudyObjective = SectionIdGenerator.StudyObjectivesIdGenerator(item);
                        newStudyObjectiveList.Add(newStudyObjective);
                    }
                    CurrentSectionsEntity currentSectionsEntity = new CurrentSectionsEntity()
                    {
                        currentSectionsId = IdGenerator.GenerateId(),
                        sectionType = StudySectionTypes.OBJECTIVES.ToString(),
                        objectives = newStudyObjectiveList
                    };
                    existing.clinicalStudy.currentSections.Add(currentSectionsEntity);
                }

                else if (existingStudyObjectivesEntities.Count() != 0 && incomingStudyObjectivesEntities.Count() != 0)
                {
                    var studyObjectivesList = new List<StudyObjectiveEntity>();
                    foreach (var item in incomingStudyObjectivesEntities)
                    {
                        if (existingStudyObjectivesEntities.Any(x => x.objectiveId == item.objectiveId))
                        {
                            if(item.endpoints!=null)
                            {
                                var endPointList = new List<EndpointsEntity>();
                                foreach (var endpoint in item.endpoints)
                                {
                                    if (existingStudyObjectivesEntities.Find(x => x.objectiveId == item.objectiveId)
                                        .endpoints.Any(x => x.endPointsId == endpoint.endPointsId))
                                    {
                                        endPointList.Add(endpoint);
                                        existingStudyObjectivesEntities.Find(x => x.objectiveId == item.objectiveId)
                                        .endpoints.RemoveAll(x => x.endPointsId == endpoint.endPointsId);
                                    }
                                    else
                                    {
                                        endpoint.endPointsId = IdGenerator.GenerateId();
                                        endPointList.Add(endpoint);
                                    }
                                }
                                item.endpoints = endPointList;
                            }                            
                            studyObjectivesList.Add(item);
                            existing.clinicalStudy.currentSections.Find(x => x.objectives != null).objectives.RemoveAll(x => x.objectiveId == item.objectiveId);
                        }
                        else
                        {
                            var studyObjective = SectionIdGenerator.StudyObjectivesIdGenerator(item);
                            studyObjectivesList.Add(studyObjective);
                        }                        
                    }
                    existing.clinicalStudy.currentSections.FindAll(x => x.objectives != null).ForEach(x => x.objectives = studyObjectivesList);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for Study Indication section 
        /// </summary>
        /// <param name="existing"></param>
        /// <param name="existingStudyIndicationEntities"></param>
        /// <param name="incomingStudyIndicationEntities"></param>
        public static void StudyIndicationSectionCheck(StudyEntity existing, List<StudyIndicationEntity> existingStudyIndicationEntities, List<StudyIndicationEntity> incomingStudyIndicationEntities)
        {
            try
            {
                if (existingStudyIndicationEntities.Count() == 0 && incomingStudyIndicationEntities.Count() != 0)
                {
                    incomingStudyIndicationEntities.ForEach(x => x.studyIndicationId = IdGenerator.GenerateId());
                    CurrentSectionsEntity currentSectionsEntity = new CurrentSectionsEntity()
                    {
                        currentSectionsId = IdGenerator.GenerateId(),
                        sectionType = StudySectionTypes.STUDY_INDICATIONS.ToString(),
                        studyIndications = incomingStudyIndicationEntities
                    };
                    existing.clinicalStudy.currentSections.Add(currentSectionsEntity);
                }

                else if (existingStudyIndicationEntities.Count() != 0 && incomingStudyIndicationEntities.Count() != 0)
                {
                    var studyIndicationList = new List<StudyIndicationEntity>();
                    foreach (var item in incomingStudyIndicationEntities)
                    {
                        if (existingStudyIndicationEntities.Any(x => x.studyIndicationId == item.studyIndicationId))
                        {
                            studyIndicationList.Add(item);
                            existing.clinicalStudy.currentSections.Find(x => x.studyIndications != null).studyIndications.RemoveAll(x => x.studyIndicationId == item.studyIndicationId);
                        }
                        else
                        {
                            item.studyIndicationId = IdGenerator.GenerateId();
                            studyIndicationList.Add(item);
                        }                        
                    }
                    existing.clinicalStudy.currentSections.FindAll(x => x.studyIndications != null).ForEach(x => x.studyIndications = studyIndicationList);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for Study Design section 
        /// </summary>
        /// <param name="existing"></param>
        /// <param name="existingStudyDesignEntities"></param>
        /// <param name="incomingStudyDesignEntities"></param>
        public static void StudyDesignSectionCheck(StudyEntity existing, List<StudyDesignEntity> existingStudyDesignEntities,List<StudyDesignEntity> incomingStudyDesignEntities)
        {
            try
            {
                if (existingStudyDesignEntities.Count() == 0 && incomingStudyDesignEntities.Count() != 0)
                {
                    var newStudyDesignList = new List<StudyDesignEntity>();
                    foreach (var item in incomingStudyDesignEntities)
                    {
                        var newStudyDesign = SectionIdGenerator.StudyDesignIdGenerator(item);
                        newStudyDesignList.Add(newStudyDesign);
                    }
                    CurrentSectionsEntity currentSectionsEntity = new CurrentSectionsEntity()
                    {
                        currentSectionsId = IdGenerator.GenerateId(),
                        sectionType = StudySectionTypes.STUDY_DESIGNS.ToString(),
                        studyDesigns = newStudyDesignList
                    };
                    existing.clinicalStudy.currentSections.Add(currentSectionsEntity);
                }
                else if (existingStudyDesignEntities.Count() != 0 && incomingStudyDesignEntities.Count() != 0)
                {
                    var studyDesignList = new List<StudyDesignEntity>();
                    foreach (var item in incomingStudyDesignEntities)
                    {
                        if (existingStudyDesignEntities.Any(x => x.studyDesignId == item.studyDesignId))
                        {                            
                            List<PlannedWorkFlowEntity> existingPlannedWorkFlowEntities = existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                     .FindAll(x => x.plannedWorkflows != null).Count() != 0 ?
                                                                                          existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                     .Find(x => x.plannedWorkflows != null).plannedWorkflows : new List<PlannedWorkFlowEntity>();
                            List<PlannedWorkFlowEntity> incomingPlannedWorkFlowEntities = incomingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                      .FindAll(x => x.plannedWorkflows != null).Count() != 0 ?
                                                                                           incomingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                      .Find(x => x.plannedWorkflows != null).plannedWorkflows : new List<PlannedWorkFlowEntity>();

                            List<StudyPopulationEntity> existingStudyPopulationsEntities = existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                     .FindAll(x => x.studyPopulations != null).Count() != 0 ?
                                                                                          existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                     .Find(x => x.studyPopulations != null).studyPopulations : new List<StudyPopulationEntity>();
                            List<StudyPopulationEntity> incomingStudyPopulationsEntities = incomingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                      .FindAll(x => x.studyPopulations != null).Count() != 0 ?
                                                                                           incomingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                      .Find(x => x.studyPopulations != null).studyPopulations : new List<StudyPopulationEntity>();

                            List<StudyCellEntity> existingStudyCellEntities = existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                     .FindAll(x => x.studyCells != null).Count() != 0 ?
                                                                                          existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                     .Find(x => x.studyCells != null).studyCells : new List<StudyCellEntity>();
                            List<StudyCellEntity> incomingStudyCellEntities = incomingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                      .FindAll(x => x.studyCells != null).Count() != 0 ?
                                                                                           incomingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                      .Find(x => x.studyCells != null).studyCells : new List<StudyCellEntity>();

                            List<InvestigationalInterventionEntity> existingInvestigationalInterventionEntities = existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                     .FindAll(x => x.studyCells != null).Count() != 0 ?
                                                                                          existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                     .Find(x => x.investigationalInterventions != null).investigationalInterventions : new List<InvestigationalInterventionEntity>();
                            List<InvestigationalInterventionEntity> incomingInvestigationalInterventionEntities = incomingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                     .FindAll(x => x.studyCells != null).Count() != 0 ?
                                                                                          incomingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId).currentSections
                                                                                                                     .Find(x => x.investigationalInterventions != null).investigationalInterventions : new List<InvestigationalInterventionEntity>();                          

                            Parallel.Invoke(
                                    //Study Populations Section
                                    ()=>StudyPopulationsSectionCheck(existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId), existingStudyPopulationsEntities, incomingStudyPopulationsEntities),
                                    //Study Cells Section
                                    ()=>StudyCellsSectionCheck(existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId), existingStudyCellEntities, incomingStudyCellEntities),
                                    //Planned WorkFlow Section
                                    ()=>StudyPlannedWorkFlowSectionCheck(existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId), existingPlannedWorkFlowEntities, incomingPlannedWorkFlowEntities),
                                    //Investigational Intervention Section
                                    ()=>InvestigationalInvestigationSectionCheck(existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId), existingInvestigationalInterventionEntities, incomingInvestigationalInterventionEntities)
                                );

                            studyDesignList.Add(existingStudyDesignEntities.Find(x => x.studyDesignId == item.studyDesignId));
                            existing.clinicalStudy.currentSections.Find(x => x.studyDesigns != null).studyDesigns.RemoveAll(x => x.studyDesignId == item.studyDesignId);
                        }
                        else
                        {
                            var studyDesign = SectionIdGenerator.StudyDesignIdGenerator(item);
                            studyDesignList.Add(studyDesign);
                        }                        
                    }                    
                    existing.clinicalStudy.currentSections.FindAll(x => x.studyDesigns != null).ForEach(x => x.studyDesigns = studyDesignList);
                }

                #region Previous and Next Items Logic
                PreviousItemNextItemHelper.PreviousItemsNextItemsWraper(existing);
                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Study Design Sections
        /// <summary>
        /// Check for Investigational Intervention section 
        /// </summary>
        /// <param name="existingStudyDesign"></param>
        /// <param name="existingInvestigationalInterventionEntities"></param>
        /// <param name="incomingInvestigationalInterventionEntities"></param>
        public static void InvestigationalInvestigationSectionCheck(StudyDesignEntity existingStudyDesign, List<InvestigationalInterventionEntity> existingInvestigationalInterventionEntities, List<InvestigationalInterventionEntity> incomingInvestigationalInterventionEntities)
        {
            try
            {
                if (existingInvestigationalInterventionEntities.Count() == 0 && incomingInvestigationalInterventionEntities.Count() != 0)
                {
                    incomingInvestigationalInterventionEntities.ForEach(x => x.investigationalInterventionId = IdGenerator.GenerateId());
                    CurrentSectionsEntity currentSectionsEntity = new CurrentSectionsEntity()
                    {
                        currentSectionsId = IdGenerator.GenerateId(),
                        sectionType = StudySectionTypes.INVESTIGATIONAL_INTERVENTIONS.ToString(),
                        investigationalInterventions = incomingInvestigationalInterventionEntities
                    };
                    existingStudyDesign.currentSections.Add(currentSectionsEntity);
                }

                else if (existingInvestigationalInterventionEntities.Count() != 0 && incomingInvestigationalInterventionEntities.Count() != 0)
                {
                    var investigationalInterventionList = new List<InvestigationalInterventionEntity>();
                    foreach (var item in incomingInvestigationalInterventionEntities)
                    {
                        if (existingInvestigationalInterventionEntities.Any(x => x.investigationalInterventionId == item.investigationalInterventionId))
                        {
                            investigationalInterventionList.Add(item);
                            existingStudyDesign.currentSections.Find(x => x.investigationalInterventions != null).investigationalInterventions.RemoveAll(x => x.investigationalInterventionId == item.investigationalInterventionId);
                        }
                        else
                        {
                            item.investigationalInterventionId = IdGenerator.GenerateId();
                            investigationalInterventionList.Add(item);
                        }
                    }
                    existingStudyDesign.currentSections.FindAll(x => x.investigationalInterventions != null).ForEach(x => x.investigationalInterventions = investigationalInterventionList);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for Study Population section 
        /// </summary>
        /// <param name="existingStudyDesign"></param>
        /// <param name="existingStudyPopulationEntities"></param>
        /// <param name="incomingStudyPopulationEntities"></param>
        public static void StudyPopulationsSectionCheck(StudyDesignEntity existingStudyDesign, List<StudyPopulationEntity> existingStudyPopulationEntities, List<StudyPopulationEntity> incomingStudyPopulationEntities)
        {
            try
            {
                if (existingStudyPopulationEntities.Count() == 0 && incomingStudyPopulationEntities.Count() != 0)
                {
                    existingStudyPopulationEntities.ForEach(x => x.studyPopulationId = IdGenerator.GenerateId());
                    CurrentSectionsEntity currentSectionsEntity = new CurrentSectionsEntity()
                    {
                        currentSectionsId = IdGenerator.GenerateId(),
                        sectionType = StudySectionTypes.STUDY_POPULATIONS.ToString(),
                        studyPopulations = existingStudyPopulationEntities
                    };
                    existingStudyDesign.currentSections.Add(currentSectionsEntity);
                }

                else if (existingStudyPopulationEntities.Count() != 0 && incomingStudyPopulationEntities.Count() != 0)
                {
                    var studyPopulationList = new List<StudyPopulationEntity>();
                    foreach (var item in incomingStudyPopulationEntities)
                    {
                        if (existingStudyPopulationEntities.Any(x => x.studyPopulationId == item.studyPopulationId))
                        {
                            studyPopulationList.Add(item);
                            existingStudyDesign.currentSections.Find(x => x.studyPopulations != null).studyPopulations.RemoveAll(x => x.studyPopulationId == item.studyPopulationId);
                        }
                        else
                        {                            
                            item.studyPopulationId = IdGenerator.GenerateId();
                            studyPopulationList.Add(item);
                        }
                    }
                    existingStudyDesign.currentSections.FindAll(x => x.studyPopulations != null).ForEach(x => x.studyPopulations = studyPopulationList);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Planned WorkFlow Section
        /// <summary>
        /// Check for Study Planned WorkFlow section 
        /// </summary>
        /// <param name="existingStudyDesign"></param>
        /// <param name="existingPlannedWorkFlowsEntities"></param>
        /// <param name="incomingPlannedWorkFlowsEntities"></param>
        public static void StudyPlannedWorkFlowSectionCheck(StudyDesignEntity existingStudyDesign, List<PlannedWorkFlowEntity> existingPlannedWorkFlowsEntities, List<PlannedWorkFlowEntity> incomingPlannedWorkFlowsEntities)
        {
            try
            {
                if (existingPlannedWorkFlowsEntities.Count() == 0 && incomingPlannedWorkFlowsEntities.Count() != 0)
                {
                    var newPlannedWorkFlowList = new List<PlannedWorkFlowEntity>();
                    foreach (var item in incomingPlannedWorkFlowsEntities)
                    {
                        var newPlannedWorkflow = SectionIdGenerator.StudyPlannedWorkFlowIdGenerator(item);
                        newPlannedWorkFlowList.Add(newPlannedWorkflow);
                    }
                    CurrentSectionsEntity currentSectionsEntity = new CurrentSectionsEntity()
                    {
                        currentSectionsId = IdGenerator.GenerateId(),
                        sectionType = StudySectionTypes.PLANNED_WORKFLOWS.ToString(),
                        plannedWorkflows = newPlannedWorkFlowList
                    };
                    existingStudyDesign.currentSections.Add(currentSectionsEntity);
                }

                else if (existingPlannedWorkFlowsEntities.Count() != 0 && incomingPlannedWorkFlowsEntities.Count() != 0)
                {
                    var plannedWorkFlowsList = new List<PlannedWorkFlowEntity>();
                    foreach (var item in incomingPlannedWorkFlowsEntities)
                    {
                        if (existingPlannedWorkFlowsEntities.Any(x => x.plannedWorkFlowId == item.plannedWorkFlowId))
                        {                            
                            PointInTimeSectionCheck(existingPlannedWorkFlowsEntities.Find(x => x.plannedWorkFlowId == item.plannedWorkFlowId).startPoint,item.startPoint);
                            PointInTimeSectionCheck(existingPlannedWorkFlowsEntities.Find(x => x.plannedWorkFlowId == item.plannedWorkFlowId).endPoint,item.endPoint);
                            WorkFlowItemMatrixSectionCheck(existingPlannedWorkFlowsEntities.Find(x => x.plannedWorkFlowId == item.plannedWorkFlowId).workflowItemMatrix,item.workflowItemMatrix);
                            //TransitionSectionCheck(existingPlannedWorkFlowsEntities.Find(x => x.plannedWorkFlowId == item.plannedWorkFlowId).transitions,item.transitions);
                            plannedWorkFlowsList.Add(item);
                            existingStudyDesign.currentSections.Find(x => x.plannedWorkflows != null).plannedWorkflows.RemoveAll(x => x.plannedWorkFlowId == item.plannedWorkFlowId);
                        }
                        else
                        {
                            var plannedWorkFlows = SectionIdGenerator.StudyPlannedWorkFlowIdGenerator(item);
                            plannedWorkFlowsList.Add(plannedWorkFlows);
                        }
                    }
                    existingStudyDesign.currentSections.FindAll(x => x.plannedWorkflows != null).ForEach(x => x.plannedWorkflows = plannedWorkFlowsList);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for PointInTime section 
        /// </summary>
        /// <param name="existingPointInTime"></param>
        /// <param name="incomingPointInTime"></param>
        public static void PointInTimeSectionCheck(PointInTimeEntity existingPointInTime, PointInTimeEntity incomingPointInTime)
        {
            try
            {
                if (existingPointInTime.pointInTimeId == null && incomingPointInTime.pointInTimeId != null)
                {
                    incomingPointInTime.pointInTimeId = IdGenerator.GenerateId();
                    existingPointInTime = incomingPointInTime;
                }
                else if (existingPointInTime.pointInTimeId != incomingPointInTime.pointInTimeId)
                {
                    incomingPointInTime.pointInTimeId = IdGenerator.GenerateId();
                    existingPointInTime = incomingPointInTime;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for WorkFlowItemMatrix section 
        /// </summary>
        /// <param name="existingWorkFlowItemMatrixEntity"></param>
        /// <param name="incomingWorkFlowItemMatrixEntity"></param>
        public static void WorkFlowItemMatrixSectionCheck(WorkFlowItemMatrixEntity existingWorkFlowItemMatrixEntity,WorkFlowItemMatrixEntity incomingWorkFlowItemMatrixEntity)
        {
            try
            {
                if ((existingWorkFlowItemMatrixEntity.workFlowItemMatrixId == null && incomingWorkFlowItemMatrixEntity.workFlowItemMatrixId != null)
                    || (existingWorkFlowItemMatrixEntity.workFlowItemMatrixId != incomingWorkFlowItemMatrixEntity.workFlowItemMatrixId))
                {
                    incomingWorkFlowItemMatrixEntity.workFlowItemMatrixId = IdGenerator.GenerateId();
                    incomingWorkFlowItemMatrixEntity.matrix.ForEach(m =>
                    {
                        m.matrixId = IdGenerator.GenerateId();
                        m.items.ForEach(item =>
                        {
                            item.itemId = IdGenerator.GenerateId();
                            item.fromPointInTime.pointInTimeId = IdGenerator.GenerateId();
                            item.toPointInTime.pointInTimeId = IdGenerator.GenerateId();
                            item.activity.activityId = IdGenerator.GenerateId();
                            item.activity.studyDataCollection.ForEach(sdc => sdc.studyDataCollectionId = IdGenerator.GenerateId());
                            item.encounter.encounterId = IdGenerator.GenerateId();
                            item.encounter.startRule.RuleId = IdGenerator.GenerateId();
                            item.encounter.endRule.RuleId = IdGenerator.GenerateId();
                            item.encounter.epoch.epochId = IdGenerator.GenerateId();
                        });
                    });
                    existingWorkFlowItemMatrixEntity = incomingWorkFlowItemMatrixEntity;
                }                
                else if (existingWorkFlowItemMatrixEntity.workFlowItemMatrixId == incomingWorkFlowItemMatrixEntity.workFlowItemMatrixId)
                {
                    MatrixSectionCheck(existingWorkFlowItemMatrixEntity.matrix, incomingWorkFlowItemMatrixEntity.matrix);
                    existingWorkFlowItemMatrixEntity = incomingWorkFlowItemMatrixEntity;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for Matrix section 
        /// </summary>
        /// <param name="existingMatrixEntities"></param>
        /// <param name="incomingMatrixEntities"></param>
        public static void MatrixSectionCheck(List<MatrixEntity> existingMatrixEntities, List<MatrixEntity> incomingMatrixEntities)
        {
            try
            {
                var matrixList = new List<MatrixEntity>();
                foreach (var iterator in incomingMatrixEntities)
                {
                    if (existingMatrixEntities.Any(x => x.matrixId == iterator.matrixId))
                    {
                        ItemSectionCheck(existingMatrixEntities.Find(x => x.matrixId == iterator.matrixId).items, iterator.items);                        
                        matrixList.Add(iterator);
                        existingMatrixEntities.RemoveAll(x => x.matrixId == iterator.matrixId);
                    }
                    else
                    {
                        iterator.matrixId = IdGenerator.GenerateId();
                        iterator.items.ForEach(item =>
                        {
                            item.itemId = IdGenerator.GenerateId();
                            item.fromPointInTime.pointInTimeId = IdGenerator.GenerateId();
                            item.toPointInTime.pointInTimeId = IdGenerator.GenerateId();
                            item.activity.activityId = IdGenerator.GenerateId();
                            item.activity.studyDataCollection.ForEach(sdc => sdc.studyDataCollectionId = IdGenerator.GenerateId());
                            item.encounter.encounterId = IdGenerator.GenerateId();
                            item.encounter.startRule.RuleId = IdGenerator.GenerateId();
                            item.encounter.endRule.RuleId = IdGenerator.GenerateId();
                            item.encounter.epoch.epochId = IdGenerator.GenerateId();
                        });
                        matrixList.Add(iterator);
                    }

                }
                incomingMatrixEntities = matrixList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for Items section 
        /// </summary>
        /// <param name="existingItemEntities"></param>
        /// <param name="incomingItemEntities"></param>
        public static void ItemSectionCheck(List<ItemEntity> existingItemEntities, List<ItemEntity> incomingItemEntities)
        {
            try
            {
                var itemList = new List<ItemEntity>();
                foreach (var iterator in incomingItemEntities)
                {
                    if (existingItemEntities.Any(x => x.itemId == iterator.itemId))
                    {
                        PointInTimeSectionCheck(existingItemEntities.Find(x => x.itemId == iterator.itemId).fromPointInTime, iterator.fromPointInTime);
                        PointInTimeSectionCheck(existingItemEntities.Find(x => x.itemId == iterator.itemId).toPointInTime, iterator.toPointInTime);
                        ActivitySectionCheck(existingItemEntities.Find(x => x.itemId == iterator.itemId).activity, iterator.activity);
                        EncounterSectionCheck(existingItemEntities.Find(x => x.itemId == iterator.itemId).encounter, iterator.encounter);
                        iterator.nextItemsInSequence = new List<string>();
                        iterator.previousItemsInSequence = new List<string>();
                        itemList.Add(iterator);
                        existingItemEntities.RemoveAll(x => x.itemId == iterator.itemId);
                    }
                    else
                    {
                        iterator.itemId = IdGenerator.GenerateId();
                        iterator.fromPointInTime.pointInTimeId = IdGenerator.GenerateId();
                        iterator.toPointInTime.pointInTimeId = IdGenerator.GenerateId();
                        iterator.activity.activityId = IdGenerator.GenerateId();
                        iterator.activity.studyDataCollection.ForEach(sdc => sdc.studyDataCollectionId = IdGenerator.GenerateId());
                        iterator.encounter.encounterId = IdGenerator.GenerateId();
                        iterator.encounter.startRule.RuleId = IdGenerator.GenerateId();
                        iterator.encounter.endRule.RuleId = IdGenerator.GenerateId();
                        iterator.encounter.epoch.epochId = IdGenerator.GenerateId();
                        iterator.nextItemsInSequence = new List<string>();                        
                        iterator.previousItemsInSequence = new List<string>();                        
                        itemList.Add(iterator);
                    }

                }                
                incomingItemEntities = itemList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for Activity section 
        /// </summary>
        /// <param name="existingActivityEntity"></param>
        /// <param name="incomingActivityEntity"></param>
        public static void ActivitySectionCheck(ActivityEntity existingActivityEntity, ActivityEntity incomingActivityEntity)
        {
            try
            {
                if ((existingActivityEntity.activityId == null && incomingActivityEntity.activityId != null)
                    || (existingActivityEntity.activityId != incomingActivityEntity.activityId))
                {
                    incomingActivityEntity.activityId = IdGenerator.GenerateId();
                    incomingActivityEntity.studyDataCollection.ForEach(sdc => sdc.studyDataCollectionId = IdGenerator.GenerateId());                    
                }
                else if (existingActivityEntity.activityId == incomingActivityEntity.activityId)
                {
                    StudyDataCollectionSectionCheck(existingActivityEntity.studyDataCollection, incomingActivityEntity.studyDataCollection);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for StudyDataCollection section 
        /// </summary>
        /// <param name="existingStudyDataCollectionEntities"></param>
        /// <param name="incomingStudyDataCollectionEntities"></param>
        public static void StudyDataCollectionSectionCheck(List<StudyDataCollectionEntity> existingStudyDataCollectionEntities, List<StudyDataCollectionEntity> incomingStudyDataCollectionEntities)
        {
            try
            {
                var studyDataCollectionList = new List<StudyDataCollectionEntity>();
                foreach (var iterator in incomingStudyDataCollectionEntities)
                {
                    if (existingStudyDataCollectionEntities.Any(x => x.studyDataCollectionId == iterator.studyDataCollectionId))
                    {
                        studyDataCollectionList.Add(iterator);
                        existingStudyDataCollectionEntities.RemoveAll(x => x.studyDataCollectionId == iterator.studyDataCollectionId);
                    }
                    else
                    {
                        iterator.studyDataCollectionId = IdGenerator.GenerateId();
                        studyDataCollectionList.Add(iterator);
                    }

                }
                incomingStudyDataCollectionEntities = studyDataCollectionList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for Encounter section 
        /// </summary>
        /// <param name="existingEncounterEntity"></param>
        /// <param name="incomingEncounterEntity"></param>
        public static void EncounterSectionCheck(EncounterEntity existingEncounterEntity, EncounterEntity incomingEncounterEntity)
        {
            try
            {
                if ((existingEncounterEntity.encounterId == null && incomingEncounterEntity.encounterId != null)
                    || (existingEncounterEntity.encounterId != incomingEncounterEntity.encounterId))
                {
                    incomingEncounterEntity.encounterId = IdGenerator.GenerateId();
                    incomingEncounterEntity.startRule.RuleId = IdGenerator.GenerateId();
                    incomingEncounterEntity.endRule.RuleId = IdGenerator.GenerateId();
                    incomingEncounterEntity.epoch.epochId = IdGenerator.GenerateId();
                }
                else if (existingEncounterEntity.encounterId == incomingEncounterEntity.encounterId)
                {
                    RuleSectionCheck(existingEncounterEntity.startRule, incomingEncounterEntity.startRule);
                    RuleSectionCheck(existingEncounterEntity.endRule, incomingEncounterEntity.endRule);
                    EpochSectionCheck(existingEncounterEntity.epoch, incomingEncounterEntity.epoch);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for Epoch section 
        /// </summary>
        /// <param name="existingEpochEntity"></param>
        /// <param name="incomingEpochEntity"></param>
        public static void EpochSectionCheck(EpochEntity existingEpochEntity, EpochEntity incomingEpochEntity)
        {
            try
            {
                if ((existingEpochEntity.epochId == null && incomingEpochEntity.epochId != null)
                    || (existingEpochEntity.epochId != incomingEpochEntity.epochId))
                {
                    incomingEpochEntity.epochId = IdGenerator.GenerateId();                    
                }               
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion        

        #region Study Cells Section
        /// <summary>
        /// Check for StudyCells section 
        /// </summary>
        /// <param name="existingStudyDesign"></param>
        /// <param name="existingStudyCellsEntities"></param>
        /// <param name="incomingStudyCellsEntities"></param>
        public static void StudyCellsSectionCheck(StudyDesignEntity existingStudyDesign, List<StudyCellEntity> existingStudyCellsEntities, List<StudyCellEntity> incomingStudyCellsEntities)
        {
            try
            {
                if (existingStudyCellsEntities.Count() == 0 && incomingStudyCellsEntities.Count() != 0)
                {
                    var newStudyCellList = new List<StudyCellEntity>();
                    foreach (var item in incomingStudyCellsEntities)
                    {
                        var newStudyCell = SectionIdGenerator.StudyCellsIdGenerator(item);
                        newStudyCellList.Add(newStudyCell);
                    }
                    CurrentSectionsEntity currentSectionsEntity = new CurrentSectionsEntity()
                    {
                        currentSectionsId = IdGenerator.GenerateId(),
                        sectionType = StudySectionTypes.STUDY_CELLS.ToString(),
                        studyCells = newStudyCellList
                    };
                    existingStudyDesign.currentSections.Add(currentSectionsEntity);
                }

                else if (existingStudyCellsEntities.Count() != 0 && incomingStudyCellsEntities.Count() != 0)
                {
                    var studyCellList = new List<StudyCellEntity>();
                    foreach (var item in incomingStudyCellsEntities)
                    {
                        if (existingStudyCellsEntities.Any(x => x.studyCellId == item.studyCellId))
                        {
                            StudyElementsSectionCheck(existingStudyCellsEntities.Find(x => x.studyCellId == item.studyCellId).studyElements, item.studyElements);
                            StudyArmSectionCheck(existingStudyCellsEntities.Find(x => x.studyCellId == item.studyCellId).studyArm, item.studyArm);
                            StudyEpochSectionCheck(existingStudyCellsEntities.Find(x => x.studyCellId == item.studyCellId).studyEpoch, item.studyEpoch);
                            studyCellList.Add(item);
                            existingStudyDesign.currentSections.Find(x => x.studyCells != null).studyCells.RemoveAll(x => x.studyCellId == item.studyCellId);
                        }
                        else
                        {
                            var studyCells = SectionIdGenerator.StudyCellsIdGenerator(item);
                            studyCellList.Add(studyCells);
                        }
                    }
                    existingStudyDesign.currentSections.FindAll(x => x.studyCells != null).ForEach(x => x.studyCells = studyCellList);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for StudyElements section 
        /// </summary>
        /// <param name="existingStudyElementsEntities"></param>
        /// <param name="incomingStudyElementsEntities"></param>
        public static void StudyElementsSectionCheck(List<StudyElementEntity> existingStudyElementsEntities, List<StudyElementEntity> incomingStudyElementsEntities)
        {
            try
            {
                var studyElementsList = new List<StudyElementEntity>();
                foreach (var item in incomingStudyElementsEntities)
                {
                    if (existingStudyElementsEntities.Any(x => x.studyElementId == item.studyElementId))
                    {
                        RuleSectionCheck(existingStudyElementsEntities.Find(x => x.studyElementId == item.studyElementId).startRule, item.startRule);
                        RuleSectionCheck(existingStudyElementsEntities.Find(x => x.studyElementId == item.studyElementId).endRule, item.endRule);
                        studyElementsList.Add(item);
                        existingStudyElementsEntities.RemoveAll(x => x.studyElementId == item.studyElementId);
                    }
                    else
                    {
                        item.studyElementId = IdGenerator.GenerateId();
                        item.startRule.RuleId = IdGenerator.GenerateId();
                        item.endRule.RuleId = IdGenerator.GenerateId();
                        studyElementsList.Add(item);
                    }
                }
                incomingStudyElementsEntities = studyElementsList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for Rule section 
        /// </summary>
        /// <param name="existingRuleEntity"></param>
        /// <param name="incomingRuleEntity"></param>
        public static void RuleSectionCheck(RuleEntity existingRuleEntity, RuleEntity incomingRuleEntity)
        {
            try
            {
                if (existingRuleEntity.RuleId == null && incomingRuleEntity.RuleId != null)
                {
                    incomingRuleEntity.RuleId = IdGenerator.GenerateId();
                }
                else if (existingRuleEntity.RuleId != incomingRuleEntity.RuleId)
                {
                    incomingRuleEntity.RuleId = IdGenerator.GenerateId();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for StudyArm section 
        /// </summary>
        /// <param name="existingStudyArm"></param>
        /// <param name="incomingStudyArm"></param>
        public static void StudyArmSectionCheck(StudyArmEntity existingStudyArm,StudyArmEntity incomingStudyArm)
        {
            try
            {
                if (existingStudyArm.studyArmId == null && incomingStudyArm.studyArmId != null)
                {
                    incomingStudyArm.studyArmId = IdGenerator.GenerateId();
                }
                else if (existingStudyArm.studyArmId != incomingStudyArm.studyArmId)
                {
                    incomingStudyArm.studyArmId = IdGenerator.GenerateId();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check for StudyEpoch section 
        /// </summary>
        /// <param name="existingStudyEpoch"></param>
        /// <param name="incomingStudyEpoch"></param>
        public static void StudyEpochSectionCheck(StudyEpochEntity existingStudyEpoch, StudyEpochEntity incomingStudyEpoch)
        {
            try
            {
                if (existingStudyEpoch.studyEpochId == null && incomingStudyEpoch.studyEpochId != null)
                {
                    incomingStudyEpoch.studyEpochId = IdGenerator.GenerateId();
                }
                else if (existingStudyEpoch.studyEpochId != incomingStudyEpoch.studyEpochId)
                {
                    incomingStudyEpoch.studyEpochId = IdGenerator.GenerateId();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #endregion

        #endregion
    }
}
