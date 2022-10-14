using ExtCore.Data.Abstractions;
using Infrastructure.Domain.Commands;
using Manufactures.Domain.GarmentSubcon.ServiceSubconFabricWashes;
using Manufactures.Domain.GarmentSubcon.ServiceSubconFabricWashes.Commands;
using Manufactures.Domain.GarmentSubcon.ServiceSubconFabricWashes.Repositories;
using Manufactures.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Manufactures.Application.GarmentSubcon.GarmentServiceSubconFabricWashes.CommandHandlers
{
    public class UpdateGarmentServiceSubconFabricWashCommandHandler : ICommandHandler<UpdateGarmentServiceSubconFabricWashCommand, GarmentServiceSubconFabricWash>
    {
        private readonly IStorage _storage;
        private readonly IGarmentServiceSubconFabricWashRepository _garmentServiceSubconFabricWashRepository;
        private readonly IGarmentServiceSubconFabricWashItemRepository _garmentServiceSubconFabricWashItemRepository;
        private readonly IGarmentServiceSubconFabricWashDetailRepository _garmentServiceSubconFabricWashDetailRepository;

        public UpdateGarmentServiceSubconFabricWashCommandHandler(IStorage storage)
        {
            _storage = storage;
            _garmentServiceSubconFabricWashRepository = _storage.GetRepository<IGarmentServiceSubconFabricWashRepository>();
            _garmentServiceSubconFabricWashItemRepository = _storage.GetRepository<IGarmentServiceSubconFabricWashItemRepository>();
            _garmentServiceSubconFabricWashDetailRepository = storage.GetRepository<IGarmentServiceSubconFabricWashDetailRepository>();
        }

        public async Task<GarmentServiceSubconFabricWash> Handle(UpdateGarmentServiceSubconFabricWashCommand request, CancellationToken cancellationToken)
        {
            var serviceSubconFabricWash = _garmentServiceSubconFabricWashRepository.Query.Where(o => o.Identity == request.Identity).Select(o => new GarmentServiceSubconFabricWash(o)).Single();

            Dictionary<Guid, double> fabricWashUpdated = new Dictionary<Guid, double>();

            _garmentServiceSubconFabricWashItemRepository.Find(o => o.ServiceSubconFabricWashId == serviceSubconFabricWash.Identity).ForEach(async subconFabricWashItem =>
            {
                var item = request.Items.Where(o => o.Id == subconFabricWashItem.Identity).SingleOrDefault();

                if (item == null)
                {
                    _garmentServiceSubconFabricWashDetailRepository.Find(i => i.ServiceSubconFabricWashItemId == subconFabricWashItem.Identity).ForEach(async subconFabricWashDetail =>
                    {
                        subconFabricWashDetail.Remove();
                        await _garmentServiceSubconFabricWashDetailRepository.Update(subconFabricWashDetail);
                    });

                    subconFabricWashItem.Remove();
                }
                else
                {
                    _garmentServiceSubconFabricWashDetailRepository.Find(i => i.ServiceSubconFabricWashItemId == subconFabricWashItem.Identity).ForEach(async subconFabricWashDetail =>
                    {
                        var detail = item.Details.Where(o => o.Id == subconFabricWashDetail.Identity).Single();
                        if (!detail.IsSave)
                        {
                            subconFabricWashDetail.Remove();
                        }
                        else
                        {
                            subconFabricWashDetail.SetQuantity(detail.Quantity);
                            subconFabricWashDetail.SetProductRemark(detail.Product.Remark);
                            subconFabricWashDetail.Modify();
                        }

                        await _garmentServiceSubconFabricWashDetailRepository.Update(subconFabricWashDetail);
                    });

                    subconFabricWashItem.Modify();
                }

                await _garmentServiceSubconFabricWashItemRepository.Update(subconFabricWashItem);
            });


            serviceSubconFabricWash.SetServiceSubconFabricWashDate(request.ServiceSubconFabricWashDate.GetValueOrDefault());
            serviceSubconFabricWash.SetRemark(request.Remark);
            serviceSubconFabricWash.SetQtyPacking(request.QtyPacking);
            serviceSubconFabricWash.SetUomUnit(request.UomUnit);
            serviceSubconFabricWash.SetNettWeight(request.NettWeight);
            serviceSubconFabricWash.SetGrossWeight(request.GrossWeight);
            serviceSubconFabricWash.Modify();
            var existingItem = _garmentServiceSubconFabricWashItemRepository.Find(o => o.ServiceSubconFabricWashId == serviceSubconFabricWash.Identity);

            var newitem = request.Items.Where(x => !existingItem.Select(o => o.UnitExpenditureNo).Contains(x.UnitExpenditureNo)).ToList();
            var removeItem = existingItem.Where(x => !request.Items.Select(o => o.UnitExpenditureNo).Contains(x.UnitExpenditureNo)).ToList();

            if (newitem.Count() > 0)
            {
                foreach (var item in newitem)
                {
                    GarmentServiceSubconFabricWashItem garmentServiceSubconFabricWashItem = new GarmentServiceSubconFabricWashItem(
                        Guid.NewGuid(),
                        serviceSubconFabricWash.Identity,
                        item.UnitExpenditureNo,
                        item.ExpenditureDate,
                        new UnitSenderId(item.UnitSender.Id),
                        item.UnitSender.Code,
                        item.UnitSender.Name,
                        new UnitRequestId(item.UnitRequest.Id),
                        item.UnitRequest.Code,
                        item.UnitRequest.Name
                    );

                    foreach (var detail in item.Details)
                    {
                        if (detail.IsSave)
                        {
                            GarmentServiceSubconFabricWashDetail garmentServiceSubconFabricWashDetail = new GarmentServiceSubconFabricWashDetail(
                                         Guid.NewGuid(),
                                         garmentServiceSubconFabricWashItem.Identity,
                                         new ProductId(detail.Product.Id),
                                         detail.Product.Code,
                                         detail.Product.Name,
                                         detail.Product.Remark,
                                         detail.DesignColor,
                                         detail.Quantity,
                                         new UomId(detail.Uom.Id),
                                         detail.Uom.Unit
                                     );
                            await _garmentServiceSubconFabricWashDetailRepository.Update(garmentServiceSubconFabricWashDetail);
                        }
                    }
                    await _garmentServiceSubconFabricWashItemRepository.Update(garmentServiceSubconFabricWashItem);
                }
            }

            if (removeItem.Count() > 0)
            {
                foreach (var item in removeItem)
                {
                    _garmentServiceSubconFabricWashDetailRepository.Find(i => i.ServiceSubconFabricWashItemId == item.Identity).ForEach(async serviceSubconFabricWashDetail =>
                    {
                        serviceSubconFabricWashDetail.Remove();
                        await _garmentServiceSubconFabricWashDetailRepository.Update(serviceSubconFabricWashDetail);
                    });
                    item.Remove();
                    await _garmentServiceSubconFabricWashItemRepository.Update(item);
                }
            }

            await _garmentServiceSubconFabricWashRepository.Update(serviceSubconFabricWash);

            _storage.Save();

            return serviceSubconFabricWash;
        }
    }
}
