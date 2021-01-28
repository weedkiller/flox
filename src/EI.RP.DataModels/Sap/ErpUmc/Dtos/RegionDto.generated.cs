using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.ErpUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class RegionDto : ODataDtoModel
    {
        public override string CollectionName() => "Regions";
        public override object[] UniqueId()
        {
            return new object[]{CountryID, RegionID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        [Filterable]
        public virtual string CountryID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        [Filterable]
        public virtual string RegionID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(20)]
        [Filterable]
        public virtual string Name
        {
            get;
            set;
        }

        = string.Empty;
    }
}