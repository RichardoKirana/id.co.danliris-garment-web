using ExtCore.Data.Abstractions;
using Infrastructure.Domain.Commands;
using Manufactures.Domain.GarmentSubcon.ServiceSubconCuttings;
using Manufactures.Domain.GarmentSubcon.ServiceSubconCuttings.Commands;
using Manufactures.Domain.GarmentSubcon.ServiceSubconCuttings.Repositories;
using Manufactures.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Manufactures.Application.GarmentSubcon.GarmentServiceSubconCuttings.CommandHandlers
{
    public class UpdateGarmentServiceSubconCuttingCommandHandler : ICommandHandler<UpdateGarmentServiceSubconCuttingCommand, GarmentServiceSubconCutting>
    {
        private readonly IStorage _storage;
        private readonly IGarmentServiceSubconCuttingRepository _garmentServiceSubconCuttingRepository;
        private readonly IGarmentServiceSubconCuttingItemRepository _garmentServiceSubconCuttingItemRepository;
        private readonly IGarmentServiceSubconCuttingDetailRepository _garmentServiceSubconCuttingDetailRepository;
        private readonly IGarmentServiceSubconCuttingSizeRepository _garmentServiceSubconCuttingSizeRepository;

        public UpdateGarmentServiceSubconCuttingCommandHandler(IStorage storage)
        {
            _storage = storage;
            _garmentServiceSubconCuttingRepository = storage.GetRepository<IGarmentServiceSubconCuttingRepository>();
            _garmentServiceSubconCuttingItemRepository = storage.GetRepository<IGarmentServiceSubconCuttingItemRepository>();
            _garmentServiceSubconCuttingDetailRepository = storage.GetRepository<IGarmentServiceSubconCuttingDetailRepository>();
            _garmentServiceSubconCuttingSizeRepository = storage.GetRepository<IGarmentServiceSubconCuttingSizeRepository>();
        }

        public async Task<GarmentServiceSubconCutting> Handle(UpdateGarmentServiceSubconCuttingCommand request, CancellationToken cancellationToken)
        {
            var subconCutting = _garmentServiceSubconCuttingRepository.Query.Where(o => o.Identity == request.Identity).Select(o => new GarmentServiceSubconCutting(o)).Single();

            _garmentServiceSubconCuttingItemRepository.Find(o => o.ServiceSubconCuttingId == subconCutting.Identity).ForEach(async subconCuttingItem =>
            {
                var item = request.Items.Where(o => o.Id == subconCuttingItem.Identity).SingleOrDefault();

                if (item == null)
                {
                    _garmentServiceSubconCuttingDetailRepository.Find(i => i.ServiceSubconCuttingItemId == subconCuttingItem.Identity).ForEach(async subconDetail =>
                    {
                        subconDetail.Remove();
                        await _garmentServiceSubconCuttingDetailRepository.Update(subconDetail);
                    });
                    subconCuttingItem.Remove();

                }
                else
                {
                    _garmentServiceSubconCuttingDetailRepository.Find(i => i.ServiceSubconCuttingItemId == subconCuttingItem.Identity).ForEach(async subconDetail =>
                    {
                        var detail = item.Details.Where(o => o.Id == subconDetail.Identity).Single();
                        if (!detail.IsSave)
                        {
                            subconDetail.Remove();
                        }
                        else
                        {
                            subconDetail.SetQuantity(detail.Quantity);
                            subconDetail.SetDesignColor(detail.DesignColor);
                            subconDetail.Modify();
                        }

                        _garmentServiceSubconCuttingSizeRepository.Find(x => x.ServiceSubconCuttingDetailId == subconDetail.Identity).ForEach(async subconCuttingSizes =>
                        {
                            var sizes = detail.Sizes.Where(s => s.ServiceSubconCuttingDetailId == subconCuttingSizes.Identity).Single();
                            //var detailSize = sizes.(s => s.Id == subconCuttingSizes.Identity).Single();
                            subconCuttingSizes.SetQuantity(sizes.Quantity);
                            //subconCuttingSizes.SetColor(subconCuttingSizes.Color);
                            //subconCuttingSizes.SetProducCode(subconCuttingSizes.ProductCode);
                            //subconCuttingSizes.SetProducName(subconCuttingSizes.ProductName);
                            //subconCuttingSizes.SetSizeName(subconCuttingSizes.SizeName);
                            subconCuttingSizes.Modify();

                            await _garmentServiceSubconCuttingSizeRepository.Update(subconCuttingSizes);
                        });

                        await _garmentServiceSubconCuttingDetailRepository.Update(subconDetail);
                    });
                    subconCuttingItem.Modify();
                }


                await _garmentServiceSubconCuttingItemRepository.Update(subconCuttingItem);
            });

            subconCutting.SetDate(request.SubconDate.GetValueOrDefault());
            subconCutting.SetBuyerId(new BuyerId(request.Buyer.Id));
            subconCutting.SetBuyerCode(request.Buyer.Code);
            subconCutting.SetBuyerName(request.Buyer.Name);
            subconCutting.SetUomId(new UomId(request.Uom.Id));
            subconCutting.SetUomUnit(request.Uom.Unit);
            subconCutting.SetQtyPacking(request.QtyPacking);
            subconCutting.SetNettWeight(request.NettWeight);
            subconCutting.SetGrossWeight(request.GrossWeight);
            subconCutting.Modify();

            var existingItem = _garmentServiceSubconCuttingItemRepository.Find(o => o.ServiceSubconCuttingId == subconCutting.Identity);

            var newItem = request.Items.Where(x => !existingItem.Select(o => o.RONo).Contains(x.RONo)).ToList();
            var removeItem = existingItem.Where(x => !request.Items.Select(o => o.RONo).Contains(x.RONo)).ToList();

            //var subconCuttingDetail = _garmentServiceSubconCuttingDetailRepository.Query.Where(o => o.Identity == request.Identity).Select(o => new GarmentServiceSubconCuttingDetail(o)).Single();

            //var existingDetail = _garmentServiceSubconCuttingDetailRepository.Find(o => o.ServiceSubconCuttingItemId == subconCuttingDetail.Identity);
            //var newItemSize = 

            if (newItem.Count() > 0)
            {
                foreach (var item in newItem)
                {
                    GarmentServiceSubconCuttingItem garmentServiceSubconCuttingItem = new GarmentServiceSubconCuttingItem(
                        Guid.NewGuid(),
                        subconCutting.Identity,
                        item.Article,
                        item.RONo,
                        new GarmentComodityId(item.Comodity.Id),
                        item.Comodity.Code,
                        item.Comodity.Name
                   );

                    foreach (var detail in item.Details)
                    {
                        if (detail.IsSave)
                        {
                            GarmentServiceSubconCuttingDetail garmentServiceSubconCuttingDetail = new GarmentServiceSubconCuttingDetail(
                                Guid.NewGuid(),
                                garmentServiceSubconCuttingItem.Identity,
                                detail.DesignColor,
                                detail.Quantity
                            );

                            foreach (var size in detail.Sizes)
                            {
                                GarmentServiceSubconCuttingSize garmentServiceSubconCuttingSize = new GarmentServiceSubconCuttingSize(
                                    Guid.NewGuid(),
                                    new SizeId(size.Size.Id),
                                    size.Size.Size,
                                    size.Quantity,
                                    new UomId(size.Uom.Id),
                                    size.Uom.Unit,
                                    size.Color,
                                    garmentServiceSubconCuttingDetail.Identity,
                                    size.CuttingInId,
                                    size.CuttingInDetailId,
                                    new ProductId(size.Product.Id),
                                    size.Product.Code,
                                    size.Product.Name
                                );

                                await _garmentServiceSubconCuttingSizeRepository.Update(garmentServiceSubconCuttingSize);
                            }

                            await _garmentServiceSubconCuttingDetailRepository.Update(garmentServiceSubconCuttingDetail);
                        }
                    }

                    await _garmentServiceSubconCuttingItemRepository.Update(garmentServiceSubconCuttingItem);
                }
            }

            if (removeItem.Count() > 0)
            {
                foreach (var item in removeItem)
                {
                    _garmentServiceSubconCuttingDetailRepository.Find(i => i.ServiceSubconCuttingItemId == item.Identity).ForEach(async subconCuttingDetail =>
                    {
                        subconCuttingDetail.Remove();



                        _garmentServiceSubconCuttingSizeRepository.Find(i => i.ServiceSubconCuttingDetailId == subconCuttingDetail.Identity).ForEach(async subconCuttingSize =>
                        {
                            subconCuttingSize.Remove();

                            await _garmentServiceSubconCuttingSizeRepository.Update(subconCuttingSize);
                        });

                        await _garmentServiceSubconCuttingDetailRepository.Update(subconCuttingDetail);
                    });
                    item.Remove();
                    await _garmentServiceSubconCuttingItemRepository.Update(item);
                }
            }

            await _garmentServiceSubconCuttingRepository.Update(subconCutting);

            _storage.Save();

            return subconCutting;
        }
    }
}