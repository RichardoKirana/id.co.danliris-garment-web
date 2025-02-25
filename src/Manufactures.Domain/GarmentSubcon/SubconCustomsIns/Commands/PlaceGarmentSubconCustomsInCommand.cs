﻿using FluentValidation;
using Infrastructure.Domain.Commands;
using Manufactures.Domain.GarmentSubcon.SubconCustomsIns.ValueObjects;
using Manufactures.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manufactures.Domain.GarmentSubcon.SubconCustomsIns.Commands
{
    public class PlaceGarmentSubconCustomsInCommand : ICommand<GarmentSubconCustomsIn>
    {
        public string BcNo { get; set; }
        public DateTimeOffset? BcDate { get; set; }
        public string BcType { get; set; }
        public string SubconType { get; set; }
        public Guid SubconContractId { get; set; }
        public string SubconContractNo { get; set; }
        public Supplier Supplier { get; set; }
        public string Remark { get; set; }
        public bool IsUsed { get; set; }
        public string SubconCategory { get; set; }
        public List<GarmentSubconCustomsInItemValueObject> Items { get; set; }
    }

    public class PlaceGarmentSubconCustomsInCommandValidator : AbstractValidator<PlaceGarmentSubconCustomsInCommand>
    {
        public PlaceGarmentSubconCustomsInCommandValidator()
        {
            RuleFor(r => r.BcNo).NotNull();
            RuleFor(r => r.BcDate).NotNull().GreaterThan(DateTimeOffset.MinValue);
            RuleFor(r => r.BcType).NotNull();
            RuleFor(r => r.SubconType).NotNull();
            RuleFor(r => r.SubconCategory).NotNull();
            RuleFor(r => r.SubconContractId).NotNull();
            RuleFor(r => r.SubconContractNo).NotNull();
            RuleFor(r => r.Supplier.Id).NotEmpty().OverridePropertyName("Supplier").When(w => w.Supplier != null);
            RuleFor(r => r.Items).NotEmpty().OverridePropertyName("Item");
            RuleFor(r => r.Items).NotEmpty().WithMessage("Data Belum Ada yang dipilih").OverridePropertyName("ItemsCount").When(s => s.Items != null);
            RuleForEach(r => r.Items).SetValidator(new GarmentSubconCustomsInItemValueObjectValidator());
        }
    }

    class GarmentSubconCustomsInItemValueObjectValidator : AbstractValidator<GarmentSubconCustomsInItemValueObject>
    {
        public GarmentSubconCustomsInItemValueObjectValidator()
        {

            RuleFor(r => r.Supplier.Id).NotEmpty().OverridePropertyName("Supplier").When(w => w.Supplier != null);
            RuleFor(r => r.DoId).NotNull();
            RuleFor(r => r.DoNo).NotNull();
            RuleFor(r => r.Quantity).GreaterThan(0).WithMessage("'Jumlah' harus lebih dari '0'.");
            RuleFor(r => r.TotalQty)
               .LessThanOrEqualTo(r => r.RemainingQuantity)
               .WithMessage(x => $"'Total Jumlah ' tidak boleh lebih dari '{x.RemainingQuantity}'.");
            RuleFor(r => r.Details).NotEmpty().OverridePropertyName("Detail");
            RuleFor(r => r.Details).NotEmpty().WithMessage("Data BC Keluar Belum Ada yang dipilih").OverridePropertyName("DetailsCount").When(s => s.Details != null);
            RuleForEach(r => r.Details).SetValidator(new GarmentSubconCustomsInDetailValueObjectValidator());
        }
    }

    public class GarmentSubconCustomsInDetailValueObjectValidator : AbstractValidator<GarmentSubconCustomsInDetailValueObject>
    {
        public GarmentSubconCustomsInDetailValueObjectValidator()
        {
            RuleFor(r => r.CustomsOutNo).NotNull();
            RuleFor(r => r.SubconCustomsOutId).NotNull();
        }
    }
}
