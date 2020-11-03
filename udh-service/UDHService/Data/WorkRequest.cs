using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace UDHService.Data
{
    [Table("WORK_REQUEST")]
    public class WorkRequest
    {
        [Key][Column("WO_NUMBER")]
        public string WorkorderNumber { get; set; }
    
        [Column("MASTER_WO_NUMBER")]
        public string MasterWorkorderNumber { get; set; }

        [Column("GIS_WR_NO")]
        public decimal? GISWorkRequestNumber { get; set; }

        [Column("WR_STATUS")]
        public string WorkRequestStatus { get; set; }

        [Column("WR_TYPE")]
        public string WorkRequestType { get; set; }

        [Column("DESIGNER_ID")]
        public string DesignerId { get; set; }

        [Column("WR_DESC")]
        public string WorkRequestDescription { get; set; }

        [Column("ENG_DIST_ID")]
        public string ENG_DIST_ID { get; set; }

        [Column("COMPANY_ID")]
        public string COMPANY_ID { get; set; }

        [Column("CUSTOMER_NAME")]
        public string CUSTOMER_NAME { get; set; }

        [Column("LOCKDOWN_YN")]
        public string LOCKDOWN_YN { get; set; }

        [Column("ADDRESS")]
        public string ADDRESS { get; set; }

        [Column("CD_DIV")]
        public string CD_DIV { get; set; }

        [Column("CD_CREWHQ")]
        public string CD_CREWHQ { get; set; }

        [Column("INIT_DATE")]
        public DateTime? INIT_DATE { get; set; }

        [Column("REQT_DATE")]
        public DateTime? REQT_DATE { get; set; }

        [Column("UDH_DESIGN_STATUS")]
        public string UDH_DESIGN_STATUS { get; set; }

        [Column("PROJ_LOC_UPPER_RIGHT_X")]
        public decimal? PROJ_LOC_UPPER_RIGHT_X { get; set; }

        [Column("PROJ_LOC_UPPER_RIGHT_Y")]
        public decimal? PROJ_LOC_UPPER_RIGHT_Y { get; set; }

        [Column("PROJ_LOC_LOWER_LEFT_X")]
        public decimal? PROJ_LOC_LOWER_LEFT_X { get; set; }

        [Column("PROJ_LOC_LOWER_LEFT_Y")]
        public decimal? PROJ_LOC_LOWER_LEFT_Y { get; set; }

        [Column("WO_POLYGON")]
        public string WO_POLYGON { get; set; }

        [Column("DWG_NAME")]
        public string DWG_NAME { get; set; }

        [Column("TAX_DISTRICT")]
        public string TAX_DISTRICT { get; set; }
        
        [Column("VAULT_MASTER_ID")]
        public decimal? VAULT_MASTER_ID { get; set; }

        [Column("PROJECT_NUMBER")]
        public string PROJECT_NUMBER { get; set; }

        [Column("SUBDIVISION")]
        public string SUBDIVISION { get; set; }

        [Column("WR_OWNER_NAME")]
        public string WorkRequestOwnerName { get; set; }

        [Column("PROJECT_ID")]
        public string PROJECT_ID { get; set; }

        [Column("HUB_ID")]
        public string HUB_ID { get; set; }

        [Column("FOLDER_ID")]
        public string FOLDER_ID { get; set; }

        [Column("ITEM_ID")]
        public string ITEM_ID { get; set; }

        [Column("PROJECT_LOCATION")] 
        // Need to change to DbGeometry
        public string PROJECT_LOCATION { get; set; }

        [Column("CUSTOMER_CONTACTED")]
        public string CUSTOMER_CONTACTED { get; set; }

        [Column("PERMITS_PULLED")]
        public DateTime? PERMITS_PULLED { get; set; }
    }
}