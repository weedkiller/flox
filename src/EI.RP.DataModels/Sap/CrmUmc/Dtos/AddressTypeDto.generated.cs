using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.CrmUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class AddressTypeDto : ODataDtoModel
    {
        public override string CollectionName() => "AddressTypes";
        public override object[] UniqueId()
        {
            return new object[]{AddressTypeID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        [Filterable]
        public virtual string AddressTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        [Filterable]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
    }
}