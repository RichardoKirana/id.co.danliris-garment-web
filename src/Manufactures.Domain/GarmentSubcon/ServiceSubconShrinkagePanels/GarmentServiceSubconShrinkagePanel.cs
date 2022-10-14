using Infrastructure.Domain;
using Manufactures.Domain.Events.GarmentSubcon;
using Manufactures.Domain.GarmentSubcon.ServiceSubconShrinkagePanels.ReadModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manufactures.Domain.GarmentSubcon.ServiceSubconShrinkagePanels
{
    public class GarmentServiceSubconShrinkagePanel : AggregateRoot<GarmentServiceSubconShrinkagePanel, GarmentServiceSubconShrinkagePanelReadModel>
    {
        public string ServiceSubconShrinkagePanelNo { get; private set; }
        public DateTimeOffset ServiceSubconShrinkagePanelDate { get; private set; }
        public string Remark { get; private set; }
        public bool IsUsed { get; private set; }
        public int QtyPacking { get; private set; }
        public string UomUnit { get; private set; }
        public double NettWeight { get; private set; }
        public double GrossWeight { get; private set; }
        public GarmentServiceSubconShrinkagePanel(Guid identity, string serviceSubconShrinkagePanelNo, DateTimeOffset serviceSubconShrinkagePanelDate, string remark, bool isUsed, int qtyPacking, string uomUnit, double nettWeight, double grossWeight) : base(identity)
        {
            Identity = identity;
            ServiceSubconShrinkagePanelNo = serviceSubconShrinkagePanelNo;
            ServiceSubconShrinkagePanelDate = serviceSubconShrinkagePanelDate;
            Remark = remark;
            IsUsed = isUsed;
            QtyPacking = qtyPacking;
            UomUnit = uomUnit;
            NettWeight = nettWeight;
            GrossWeight = grossWeight;

            ReadModel = new GarmentServiceSubconShrinkagePanelReadModel(Identity)
            {
                ServiceSubconShrinkagePanelNo = ServiceSubconShrinkagePanelNo,
                ServiceSubconShrinkagePanelDate = ServiceSubconShrinkagePanelDate,
                Remark = Remark,
                IsUsed = IsUsed,
                QtyPacking = QtyPacking,
                UomUnit = UomUnit,
                NettWeight = NettWeight,
                GrossWeight = GrossWeight
            };

            ReadModel.AddDomainEvent(new OnGarmentServiceSubconShrinkagePanelPlaced(Identity));
        }

        public GarmentServiceSubconShrinkagePanel(GarmentServiceSubconShrinkagePanelReadModel readModel) : base(readModel)
        {
            ServiceSubconShrinkagePanelNo = readModel.ServiceSubconShrinkagePanelNo;
            ServiceSubconShrinkagePanelDate = readModel.ServiceSubconShrinkagePanelDate;
            Remark = readModel.Remark;
            IsUsed = readModel.IsUsed;
            QtyPacking = readModel.QtyPacking;
            UomUnit = readModel.UomUnit;
            NettWeight = readModel.NettWeight;
            GrossWeight = readModel.GrossWeight;
        }

        public void SetServiceSubconShrinkagePanelDate(DateTimeOffset ServiceSubconShrinkagePanelDate)
        {
            if (this.ServiceSubconShrinkagePanelDate != ServiceSubconShrinkagePanelDate)
            {
                this.ServiceSubconShrinkagePanelDate = ServiceSubconShrinkagePanelDate;
                ReadModel.ServiceSubconShrinkagePanelDate = ServiceSubconShrinkagePanelDate;
            }
        }

        public void SetRemark(string remark)
        {
            if (remark != Remark)
            {
                Remark = remark;
                ReadModel.Remark = remark;

                MarkModified();
            }
        }

        public void SetIsUsed(bool isUsed)
        {
            if (isUsed != IsUsed)
            {
                IsUsed = isUsed;
                ReadModel.IsUsed = isUsed;

                MarkModified();
            }
        }

        public void SetQtyPacking(int qtyPacking)
        {
            if (this.QtyPacking != qtyPacking)
            {
                this.QtyPacking = qtyPacking;
                ReadModel.QtyPacking = qtyPacking;
            }
        }

        public void SetUomUnit(string uomUnit)
        {
            if (this.UomUnit != uomUnit)
            {
                this.UomUnit = uomUnit;
                ReadModel.UomUnit = UomUnit;
            }
        }

        public void SetNettWeight(double nettWeight)
        {
            if(this.NettWeight != nettWeight)
            {
                this.NettWeight = nettWeight;
                ReadModel.NettWeight = NettWeight;
            }
        }

        public void SetGrossWeight(double grossWeight)
        {
            if(this.GrossWeight != grossWeight)
            {
                this.GrossWeight = grossWeight;
                ReadModel.GrossWeight = GrossWeight;
            }
        }

        public void Modify()
        {
            MarkModified();
        }

        protected override GarmentServiceSubconShrinkagePanel GetEntity()
        {
            return this;
        }
    }
}
