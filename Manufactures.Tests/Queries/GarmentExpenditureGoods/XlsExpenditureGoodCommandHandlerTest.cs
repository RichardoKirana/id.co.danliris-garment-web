﻿using Infrastructure.External.DanLirisClient.Microservice.HttpClientService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Infrastructure.External.DanLirisClient.Microservice.MasterResult.CostCalculationGarmentDataProductionReport;
using Manufactures.Domain.GarmentExpenditureGoods.Repositories;
using Barebone.Tests;
using Moq;
using System.Net.Http;
using System.Net;
using Manufactures.Application.GarmentExpenditureGoods.Queries;
using Xunit;
using Manufactures.Domain.GarmentExpenditureGoods.ReadModels;
using Manufactures.Domain.GarmentExpenditureGoods;
using Manufactures.Domain.Shared.ValueObjects;
using FluentAssertions;

namespace Manufactures.Tests.Queries.GarmentExpenditureGoods
{
	public class XlsExpenditureGoodCommandHandlerTest : BaseCommandUnitTest
	{
		private readonly Mock<IGarmentExpenditureGoodRepository> _mockGarmentExpenditureGoodRepository;
		private readonly Mock<IGarmentExpenditureGoodItemRepository> _mockGarmentExpenditureGoodItemRepository;
		protected readonly Mock<IHttpClientService> _mockhttpService;
		private Mock<IServiceProvider> serviceProviderMock;
		public XlsExpenditureGoodCommandHandlerTest()
		{
			_mockGarmentExpenditureGoodRepository = CreateMock<IGarmentExpenditureGoodRepository>();
			_mockGarmentExpenditureGoodItemRepository = CreateMock<IGarmentExpenditureGoodItemRepository>();

			_MockStorage.SetupStorage(_mockGarmentExpenditureGoodRepository);
			_MockStorage.SetupStorage(_mockGarmentExpenditureGoodItemRepository);

			serviceProviderMock = new Mock<IServiceProvider>();
			_mockhttpService = CreateMock<IHttpClientService>();

			List<CostCalViewModel> costCalViewModels = new List<CostCalViewModel> {
				new CostCalViewModel
				{
					ro="ro",
					comodityName="",
					buyerCode="",
					hours=10
				}
			};

			_mockhttpService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\": " + JsonConvert.SerializeObject(costCalViewModels) + "}") });
			serviceProviderMock.Setup(x => x.GetService(typeof(IHttpClientService))).Returns(_mockhttpService.Object);
		}
		private GetXlsExpenditureGoodQueryHandler CreateGetXlsExpenditureGoodQueryHandler()
		{
			return new GetXlsExpenditureGoodQueryHandler(_MockStorage.Object, serviceProviderMock.Object);
		}
		[Fact]
		public async Task Handle_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			GetXlsExpenditureGoodQueryHandler unitUnderTest = CreateGetXlsExpenditureGoodQueryHandler();
			CancellationToken cancellationToken = CancellationToken.None;

			Guid guidExpenditureGood = Guid.NewGuid();
			Guid guidExpenditureGoodItem = Guid.NewGuid();
			Guid guidSewingOut = Guid.NewGuid();
			Guid guidSewingOutItem = Guid.NewGuid();

			GetXlsExpenditureGoodQuery getMonitoring = new GetXlsExpenditureGoodQuery(1, 25, "{}", 1, DateTime.Now, DateTime.Now.AddDays(2),"", "token");

			_mockGarmentExpenditureGoodItemRepository
				.Setup(s => s.Query)
				.Returns(new List<GarmentExpenditureGoodItemReadModel>
				{
					new GarmentExpenditureGoodItem(guidExpenditureGoodItem,guidExpenditureGood,new Guid(),new SizeId(1),"",10,0,new UomId(1),"","",10,10).GetReadModel()
				}.AsQueryable());

			_mockGarmentExpenditureGoodRepository
				.Setup(s => s.Query)
				.Returns(new List<GarmentExpenditureGoodReadModel>
				{
					new GarmentExpenditureGood(guidExpenditureGood,"","",new UnitDepartmentId(1),"","","ro","",new GarmentComodityId(1),"","",new BuyerId(1),"","",DateTimeOffset.Now,"","",10,"",true).GetReadModel()
				}.AsQueryable());


			// Act
			var result = await unitUnderTest.Handle(getMonitoring, cancellationToken);

			// Assert
			result.Should().NotBeNull();
		}
		[Fact]
		public async Task Handle_StateUnderTest_ExpectedBehaviorbookkeeping()
		{
			// Arrange
			GetXlsExpenditureGoodQueryHandler unitUnderTest = CreateGetXlsExpenditureGoodQueryHandler();
			CancellationToken cancellationToken = CancellationToken.None;

			Guid guidExpenditureGood = Guid.NewGuid();
			Guid guidExpenditureGoodItem = Guid.NewGuid();
			Guid guidSewingOut = Guid.NewGuid();
			Guid guidSewingOutItem = Guid.NewGuid();

			GetXlsExpenditureGoodQuery getMonitoring = new GetXlsExpenditureGoodQuery(1, 25, "{}", 1, DateTime.Now, DateTime.Now.AddDays(2), "bookkeeping", "token");

			_mockGarmentExpenditureGoodItemRepository
				.Setup(s => s.Query)
				.Returns(new List<GarmentExpenditureGoodItemReadModel>
				{
					new GarmentExpenditureGoodItem(guidExpenditureGoodItem,guidExpenditureGood,new Guid(),new SizeId(1),"",10,0,new UomId(1),"","",10,10).GetReadModel()
				}.AsQueryable());

			_mockGarmentExpenditureGoodRepository
				.Setup(s => s.Query)
				.Returns(new List<GarmentExpenditureGoodReadModel>
				{
					new GarmentExpenditureGood(guidExpenditureGood,"","",new UnitDepartmentId(1),"","","ro","",new GarmentComodityId(1),"","",new BuyerId(1),"","",DateTimeOffset.Now,"","",10,"",true).GetReadModel()
				}.AsQueryable());


			// Act
			var result = await unitUnderTest.Handle(getMonitoring, cancellationToken);

			// Assert
			result.Should().NotBeNull();
		}
	}
}
