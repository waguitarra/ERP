-- MySQL dump 10.13  Distrib 8.0.44, for Linux (x86_64)
--
-- Host: localhost    Database: logistics_db
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Companies`
--

DROP TABLE IF EXISTS `Companies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Companies` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Document` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Companies_Document` (`Document`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Companies`
--

LOCK TABLES `Companies` WRITE;
/*!40000 ALTER TABLE `Companies` DISABLE KEYS */;
INSERT INTO `Companies` VALUES ('a1b2c3d4-5555-1111-2222-333333333333','Nexus Logistics','12345678000190',1,'2026-01-31 22:02:21.000000',NULL);
/*!40000 ALTER TABLE `Companies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Customers`
--

DROP TABLE IF EXISTS `Customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Customers` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Document` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Address` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `City` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Country` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Latitude` double DEFAULT NULL,
  `Longitude` double DEFAULT NULL,
  `State` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ZipCode` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Customers_Document` (`Document`),
  KEY `IX_Customers_CompanyId` (`CompanyId`),
  CONSTRAINT `FK_Customers_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Customers`
--

LOCK TABLES `Customers` WRITE;
/*!40000 ALTER TABLE `Customers` DISABLE KEYS */;
INSERT INTO `Customers` VALUES ('33333333-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','Cliente Premium LTDA','11111111000111','(11) 3333-1111','contato@clientepremium.com',NULL,1,'2026-01-31 22:02:21.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL),('33333333-2222-2222-2222-222222222222','a1b2c3d4-5555-1111-2222-333333333333','Loja Central S.A.','22222222000122','(11) 3333-2222','compras@lojacentral.com',NULL,1,'2026-01-31 22:02:21.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL),('33333333-3333-3333-3333-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Mercado Bom Preço','33333333000133','(11) 3333-3333','pedidos@mercadobom.com',NULL,1,'2026-01-31 22:02:21.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL),('a1b2c3d4-0002-0001-0001-000000000001','a1b2c3d4-5555-1111-2222-333333333333','Loja Central Lisboa','44444444000144','11999990004','central@loja.pt','Av. Paulista, 1000',1,'2026-01-31 22:04:59.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL),('a1b2c3d4-0002-0001-0001-000000000002','a1b2c3d4-5555-1111-2222-333333333333','Supermercado Porto Norte','55555555000155','11999990005','norte@super.pt','Rua Oscar Freire, 500',1,'2026-01-31 22:04:59.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL),('a1b2c3d4-0002-0001-0001-000000000003','a1b2c3d4-5555-1111-2222-333333333333','Distribuidora Algarve','66666666000166','11999990006','algarve@dist.pt','Av. Madrid, 2000',1,'2026-01-31 22:04:59.000000','2026-01-31 22:55:32.935627',NULL,NULL,NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `Customers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CycleCounts`
--

DROP TABLE IF EXISTS `CycleCounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CycleCounts` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CountNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `WarehouseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ZoneId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `CountDate` datetime(6) NOT NULL,
  `Status` int NOT NULL,
  `CountedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_CycleCounts_WarehouseId` (`WarehouseId`),
  KEY `IX_CycleCounts_ZoneId` (`ZoneId`),
  CONSTRAINT `FK_CycleCounts_Warehouses_WarehouseId` FOREIGN KEY (`WarehouseId`) REFERENCES `Warehouses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CycleCounts_WarehouseZones_ZoneId` FOREIGN KEY (`ZoneId`) REFERENCES `WarehouseZones` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CycleCounts`
--

LOCK TABLES `CycleCounts` WRITE;
/*!40000 ALTER TABLE `CycleCounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `CycleCounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `DockDoors`
--

DROP TABLE IF EXISTS `DockDoors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DockDoors` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `WarehouseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DoorNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Type` int NOT NULL,
  `Status` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_DockDoors_WarehouseId` (`WarehouseId`),
  CONSTRAINT `FK_DockDoors_Warehouses_WarehouseId` FOREIGN KEY (`WarehouseId`) REFERENCES `Warehouses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `DockDoors`
--

LOCK TABLES `DockDoors` WRITE;
/*!40000 ALTER TABLE `DockDoors` DISABLE KEYS */;
INSERT INTO `DockDoors` VALUES ('10000001-1111-1111-1111-111111111111','11111111-1111-1111-1111-111111111111','DOCK-01',0,0,1,'2026-01-31 22:19:23.000000',NULL),('10000001-2222-2222-2222-222222222222','11111111-1111-1111-1111-111111111111','DOCK-02',0,1,1,'2026-01-31 22:19:23.000000',NULL),('10000001-3333-3333-3333-333333333333','11111111-1111-1111-1111-111111111111','DOCK-03',1,0,1,'2026-01-31 22:19:23.000000',NULL),('10000001-4444-4444-4444-444444444444','11111111-1111-1111-1111-111111111111','DOCK-04',1,0,1,'2026-01-31 22:19:23.000000',NULL),('10000001-5555-5555-5555-555555555555','11111111-1111-1111-1111-111111111111','DOCK-05',2,0,1,'2026-01-31 22:19:23.000000',NULL),('a1b2c3d4-1020-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','DOCK-01',0,0,1,'2026-01-31 22:11:06.000000',NULL),('a1b2c3d4-1021-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','DOCK-02',0,0,1,'2026-01-31 22:11:06.000000',NULL),('a1b2c3d4-1022-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','DOCK-03',1,0,1,'2026-01-31 22:11:06.000000',NULL),('a1b2c3d4-1023-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','DOCK-04',1,0,1,'2026-01-31 22:11:06.000000',NULL);
/*!40000 ALTER TABLE `DockDoors` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Drivers`
--

DROP TABLE IF EXISTS `Drivers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Drivers` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LicenseNumber` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Drivers_LicenseNumber` (`LicenseNumber`),
  KEY `IX_Drivers_CompanyId` (`CompanyId`),
  CONSTRAINT `FK_Drivers_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Drivers`
--

LOCK TABLES `Drivers` WRITE;
/*!40000 ALTER TABLE `Drivers` DISABLE KEYS */;
INSERT INTO `Drivers` VALUES ('44444444-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','Carlos Santos','CNH12345678901','(11) 99999-1001',1,'2026-01-31 22:02:37.000000',NULL),('44444444-2222-2222-2222-222222222222','a1b2c3d4-5555-1111-2222-333333333333','Roberto Lima','CNH12345678902','(11) 99999-1002',1,'2026-01-31 22:02:37.000000',NULL),('44444444-3333-3333-3333-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Anderson Pereira','CNH12345678903','(11) 99999-1003',1,'2026-01-31 22:02:37.000000',NULL),('a1b2c3d4-4001-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','João Pedro Oliveira','CNH98765432109','11988776655',1,'2026-01-31 22:12:12.000000',NULL),('a1b2c3d4-4002-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Maria Aparecida Costa','CNH55566677788','11977665544',1,'2026-01-31 22:12:12.000000',NULL),('a1b2c3d4-4003-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Roberto Almeida Junior','CNH11122233344','11966554433',1,'2026-01-31 22:12:12.000000',NULL),('a1b2c3d4-4004-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Ana Paula Ferreira','CNH99988877766','11955443322',1,'2026-01-31 22:12:12.000000',NULL);
/*!40000 ALTER TABLE `Drivers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `InboundCartonItems`
--

DROP TABLE IF EXISTS `InboundCartonItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `InboundCartonItems` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `InboundCartonId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SKU` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SerialNumber` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsReceived` tinyint(1) NOT NULL,
  `ReceivedAt` datetime(6) DEFAULT NULL,
  `ReceivedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_InboundCartonItems_InboundCartonId` (`InboundCartonId`),
  KEY `IX_InboundCartonItems_ProductId` (`ProductId`),
  KEY `IX_InboundCartonItems_SerialNumber` (`SerialNumber`),
  KEY `IX_InboundCartonItems_SKU` (`SKU`),
  CONSTRAINT `FK_InboundCartonItems_InboundCartons_InboundCartonId` FOREIGN KEY (`InboundCartonId`) REFERENCES `InboundCartons` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_InboundCartonItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `InboundCartonItems`
--

LOCK TABLES `InboundCartonItems` WRITE;
/*!40000 ALTER TABLE `InboundCartonItems` DISABLE KEYS */;
/*!40000 ALTER TABLE `InboundCartonItems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `InboundCartons`
--

DROP TABLE IF EXISTS `InboundCartons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `InboundCartons` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `InboundParcelId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CartonNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Barcode` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `SequenceNumber` int NOT NULL,
  `TotalCartons` int NOT NULL,
  `Weight` decimal(18,2) NOT NULL,
  `Length` decimal(18,2) NOT NULL,
  `Width` decimal(18,2) NOT NULL,
  `Height` decimal(18,2) NOT NULL,
  `Status` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ReceivedAt` datetime(6) DEFAULT NULL,
  `ReceivedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `HasDamage` tinyint(1) NOT NULL,
  `DamageNotes` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_InboundCartons_Barcode` (`Barcode`),
  KEY `IX_InboundCartons_InboundParcelId` (`InboundParcelId`),
  KEY `IX_InboundCartons_Status` (`Status`),
  CONSTRAINT `FK_InboundCartons_InboundParcels_InboundParcelId` FOREIGN KEY (`InboundParcelId`) REFERENCES `InboundParcels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `InboundCartons`
--

LOCK TABLES `InboundCartons` WRITE;
/*!40000 ALTER TABLE `InboundCartons` DISABLE KEYS */;
/*!40000 ALTER TABLE `InboundCartons` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `InboundParcelItems`
--

DROP TABLE IF EXISTS `InboundParcelItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `InboundParcelItems` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `InboundParcelId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SKU` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ExpectedQuantity` decimal(18,2) NOT NULL,
  `ReceivedQuantity` decimal(18,2) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_InboundParcelItems_InboundParcelId` (`InboundParcelId`),
  KEY `IX_InboundParcelItems_ProductId` (`ProductId`),
  KEY `IX_InboundParcelItems_SKU` (`SKU`),
  CONSTRAINT `FK_InboundParcelItems_InboundParcels_InboundParcelId` FOREIGN KEY (`InboundParcelId`) REFERENCES `InboundParcels` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_InboundParcelItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `InboundParcelItems`
--

LOCK TABLES `InboundParcelItems` WRITE;
/*!40000 ALTER TABLE `InboundParcelItems` DISABLE KEYS */;
/*!40000 ALTER TABLE `InboundParcelItems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `InboundParcels`
--

DROP TABLE IF EXISTS `InboundParcels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `InboundParcels` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `InboundShipmentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ParcelNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LPN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `SequenceNumber` int NOT NULL,
  `TotalParcels` int NOT NULL,
  `ParentParcelId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `Weight` decimal(18,2) NOT NULL,
  `Length` decimal(18,2) NOT NULL,
  `Width` decimal(18,2) NOT NULL,
  `Height` decimal(18,2) NOT NULL,
  `DimensionUnit` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Status` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CurrentLocation` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ReceivedAt` datetime(6) DEFAULT NULL,
  `ReceivedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `HasDamage` tinyint(1) NOT NULL,
  `DamageNotes` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_InboundParcels_LPN` (`LPN`),
  KEY `IX_InboundParcels_InboundShipmentId` (`InboundShipmentId`),
  KEY `IX_InboundParcels_ParcelNumber` (`ParcelNumber`),
  KEY `IX_InboundParcels_ParentParcelId` (`ParentParcelId`),
  KEY `IX_InboundParcels_Status` (`Status`),
  CONSTRAINT `FK_InboundParcels_InboundParcels_ParentParcelId` FOREIGN KEY (`ParentParcelId`) REFERENCES `InboundParcels` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_InboundParcels_InboundShipments_InboundShipmentId` FOREIGN KEY (`InboundShipmentId`) REFERENCES `InboundShipments` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `InboundParcels`
--

LOCK TABLES `InboundParcels` WRITE;
/*!40000 ALTER TABLE `InboundParcels` DISABLE KEYS */;
/*!40000 ALTER TABLE `InboundParcels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `InboundShipments`
--

DROP TABLE IF EXISTS `InboundShipments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `InboundShipments` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ShipmentNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `OrderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SupplierId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `VehicleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DriverId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `ExpectedArrivalDate` datetime(6) DEFAULT NULL,
  `ActualArrivalDate` datetime(6) DEFAULT NULL,
  `DockDoorNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Status` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TotalQuantityExpected` decimal(18,2) NOT NULL,
  `TotalQuantityReceived` decimal(18,2) NOT NULL,
  `ASNNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `HasQualityIssues` tinyint(1) NOT NULL,
  `InspectedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `ReceivedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_InboundShipments_ShipmentNumber` (`ShipmentNumber`),
  KEY `IX_InboundShipments_CompanyId` (`CompanyId`),
  KEY `IX_InboundShipments_DriverId` (`DriverId`),
  KEY `IX_InboundShipments_OrderId` (`OrderId`),
  KEY `IX_InboundShipments_SupplierId` (`SupplierId`),
  KEY `IX_InboundShipments_VehicleId` (`VehicleId`),
  KEY `IX_InboundShipments_ASNNumber` (`ASNNumber`),
  KEY `IX_InboundShipments_Status` (`Status`),
  CONSTRAINT `FK_InboundShipments_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_InboundShipments_Drivers_DriverId` FOREIGN KEY (`DriverId`) REFERENCES `Drivers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_InboundShipments_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_InboundShipments_Suppliers_SupplierId` FOREIGN KEY (`SupplierId`) REFERENCES `Suppliers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_InboundShipments_Vehicles_VehicleId` FOREIGN KEY (`VehicleId`) REFERENCES `Vehicles` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `InboundShipments`
--

LOCK TABLES `InboundShipments` WRITE;
/*!40000 ALTER TABLE `InboundShipments` DISABLE KEYS */;
INSERT INTO `InboundShipments` VALUES ('b0000001-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','IS-2026-0001','88888888-2222-2222-2222-222222222222','22222222-1111-1111-1111-111111111111','55555555-1111-1111-1111-111111111111','44444444-1111-1111-1111-111111111111','2026-02-02 22:17:38.000000',NULL,'DOCK-01','Scheduled',50.00,0.00,'ASN-2026-0001',0,NULL,NULL,'2026-01-31 22:17:38.000000',NULL),('b0000001-2222-2222-2222-222222222222','a1b2c3d4-5555-1111-2222-333333333333','IS-2026-0002','88888888-4444-4444-4444-444444444444','22222222-2222-2222-2222-222222222222','55555555-2222-2222-2222-222222222222','44444444-2222-2222-2222-222222222222','2026-02-04 22:17:38.000000',NULL,'DOCK-02','InTransit',100.00,0.00,'ASN-2026-0002',0,NULL,NULL,'2026-01-31 22:17:38.000000',NULL),('b0000001-3333-3333-3333-333333333333','a1b2c3d4-5555-1111-2222-333333333333','IS-2026-0003','88888888-2222-2222-2222-222222222222','22222222-3333-3333-3333-333333333333','55555555-3333-3333-3333-333333333333','44444444-3333-3333-3333-333333333333','2026-01-31 22:17:38.000000','2026-01-31 22:17:38.000000','DOCK-03','Receiving',30.00,20.00,'ASN-2026-0003',0,NULL,NULL,'2026-01-31 22:17:38.000000',NULL);
/*!40000 ALTER TABLE `InboundShipments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Inventories`
--

DROP TABLE IF EXISTS `Inventories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Inventories` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `StorageLocationId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Quantity` decimal(65,30) NOT NULL,
  `ReservedQuantity` decimal(65,30) NOT NULL,
  `LastUpdatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Inventories_ProductId` (`ProductId`),
  KEY `IX_Inventories_StorageLocationId` (`StorageLocationId`),
  CONSTRAINT `FK_Inventories_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Inventories_StorageLocations_StorageLocationId` FOREIGN KEY (`StorageLocationId`) REFERENCES `StorageLocations` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Inventories`
--

LOCK TABLES `Inventories` WRITE;
/*!40000 ALTER TABLE `Inventories` DISABLE KEYS */;
INSERT INTO `Inventories` VALUES ('a1b2c3d4-9000-1111-2222-333333333333','a1b2c3d4-0003-0001-0001-000000000001','a1b2c3d4-1010-1111-2222-333333333333',25.000000000000000000000000000000,5.000000000000000000000000000000,'2026-01-31 22:16:06.000000'),('a1b2c3d4-9001-1111-2222-333333333333','a1b2c3d4-0003-0001-0001-000000000002','a1b2c3d4-1011-1111-2222-333333333333',15.000000000000000000000000000000,3.000000000000000000000000000000,'2026-01-31 22:16:06.000000'),('a1b2c3d4-9002-1111-2222-333333333333','a1b2c3d4-0003-0001-0001-000000000003','a1b2c3d4-1012-1111-2222-333333333333',50.000000000000000000000000000000,10.000000000000000000000000000000,'2026-01-31 22:16:06.000000'),('a1b2c3d4-9003-1111-2222-333333333333','a1b2c3d4-0003-0001-0001-000000000004','a1b2c3d4-1013-1111-2222-333333333333',40.000000000000000000000000000000,8.000000000000000000000000000000,'2026-01-31 22:16:06.000000'),('a1b2c3d4-9004-1111-2222-333333333333','a1b2c3d4-0003-0001-0001-000000000005','a1b2c3d4-1014-1111-2222-333333333333',20.000000000000000000000000000000,5.000000000000000000000000000000,'2026-01-31 22:16:06.000000'),('a1b2c3d4-9005-1111-2222-333333333333','a1b2c3d4-0003-0001-0001-000000000006','a1b2c3d4-1010-1111-2222-333333333333',100.000000000000000000000000000000,20.000000000000000000000000000000,'2026-01-31 22:16:06.000000'),('a1b2c3d4-9006-1111-2222-333333333333','a1b2c3d4-0003-0001-0001-000000000007','a1b2c3d4-1011-1111-2222-333333333333',30.000000000000000000000000000000,5.000000000000000000000000000000,'2026-01-31 22:16:06.000000'),('a1b2c3d4-9007-1111-2222-333333333333','a1b2c3d4-0003-0001-0001-000000000008','a1b2c3d4-1012-1111-2222-333333333333',35.000000000000000000000000000000,10.000000000000000000000000000000,'2026-01-31 22:16:06.000000'),('f0000001-1111-1111-1111-111111111111','77777777-1111-1111-1111-111111111111','e0000001-1111-1111-1111-111111111111',50.000000000000000000000000000000,5.000000000000000000000000000000,'2026-01-31 22:19:03.000000'),('f0000001-2222-2222-2222-222222222222','77777777-2222-2222-2222-222222222222','e0000001-1111-1111-1111-111111111111',20.000000000000000000000000000000,2.000000000000000000000000000000,'2026-01-31 22:19:03.000000'),('f0000001-3333-3333-3333-333333333333','77777777-3333-3333-3333-333333333333','e0000001-2222-2222-2222-222222222222',100.000000000000000000000000000000,10.000000000000000000000000000000,'2026-01-31 22:19:03.000000'),('f0000001-4444-4444-4444-444444444444','77777777-4444-4444-4444-444444444444','e0000001-5555-5555-5555-555555555555',500.000000000000000000000000000000,50.000000000000000000000000000000,'2026-01-31 22:19:03.000000'),('f0000001-5555-5555-5555-555555555555','77777777-5555-5555-5555-555555555555','e0000001-3333-3333-3333-333333333333',200.000000000000000000000000000000,20.000000000000000000000000000000,'2026-01-31 22:19:03.000000');
/*!40000 ALTER TABLE `Inventories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Lots`
--

DROP TABLE IF EXISTS `Lots`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Lots` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `LotNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ManufactureDate` datetime(6) NOT NULL,
  `ExpiryDate` datetime(6) NOT NULL,
  `QuantityReceived` decimal(18,2) NOT NULL,
  `QuantityAvailable` decimal(18,2) NOT NULL,
  `Status` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SupplierId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Lots_CompanyId_LotNumber` (`CompanyId`,`LotNumber`),
  KEY `IX_Lots_ExpiryDate` (`ExpiryDate`),
  KEY `IX_Lots_ProductId` (`ProductId`),
  KEY `IX_Lots_Status` (`Status`),
  KEY `IX_Lots_SupplierId` (`SupplierId`),
  CONSTRAINT `FK_Lots_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Lots_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Lots_Suppliers_SupplierId` FOREIGN KEY (`SupplierId`) REFERENCES `Suppliers` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Lots`
--

LOCK TABLES `Lots` WRITE;
/*!40000 ALTER TABLE `Lots` DISABLE KEYS */;
/*!40000 ALTER TABLE `Lots` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `OrderDocuments`
--

DROP TABLE IF EXISTS `OrderDocuments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `OrderDocuments` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `OrderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FileName` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FilePath` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FileUrl` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FileSizeBytes` bigint NOT NULL,
  `MimeType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UploadedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UploadedAt` datetime(6) NOT NULL,
  `DeletedAt` datetime(6) DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OrderDocuments_OrderId` (`OrderId`),
  KEY `IX_OrderDocuments_Type` (`Type`),
  KEY `IX_OrderDocuments_UploadedAt` (`UploadedAt`),
  CONSTRAINT `FK_OrderDocuments_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `OrderDocuments`
--

LOCK TABLES `OrderDocuments` WRITE;
/*!40000 ALTER TABLE `OrderDocuments` DISABLE KEYS */;
/*!40000 ALTER TABLE `OrderDocuments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `OrderItems`
--

DROP TABLE IF EXISTS `OrderItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `OrderItems` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `OrderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SKU` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `QuantityOrdered` decimal(18,2) NOT NULL,
  `QuantityAllocated` decimal(18,2) NOT NULL,
  `QuantityPicked` decimal(18,2) NOT NULL,
  `QuantityShipped` decimal(18,2) NOT NULL,
  `UnitPrice` decimal(18,2) NOT NULL,
  `RequiredLotNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `RequiredShipDate` datetime(6) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OrderItems_OrderId` (`OrderId`),
  KEY `IX_OrderItems_ProductId` (`ProductId`),
  CONSTRAINT `FK_OrderItems_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_OrderItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `OrderItems`
--

LOCK TABLES `OrderItems` WRITE;
/*!40000 ALTER TABLE `OrderItems` DISABLE KEYS */;
INSERT INTO `OrderItems` VALUES ('20000001-1111-1111-1111-111111111111','88888888-1111-1111-1111-111111111111','77777777-1111-1111-1111-111111111111','SKU-001',5.00,5.00,3.00,0.00,1100.00,'LOT-001','2026-02-03 22:19:54.000000','2026-01-31 22:19:54.000000',NULL),('20000001-2222-2222-2222-222222222222','88888888-2222-2222-2222-222222222222','77777777-2222-2222-2222-222222222222','SKU-002',25.00,20.00,10.00,0.00,480.00,'LOT-002','2026-02-05 22:19:54.000000','2026-01-31 22:19:54.000000',NULL),('20000001-3333-3333-3333-333333333333','88888888-3333-3333-3333-333333333333','77777777-5555-5555-5555-555555555555','SKU-005',5.00,5.00,5.00,5.00,640.00,NULL,'2026-02-02 22:19:54.000000','2026-01-31 22:19:54.000000',NULL),('20000001-4444-4444-4444-444444444444','88888888-4444-4444-4444-444444444444','77777777-3333-3333-3333-333333333333','SKU-003',100.00,80.00,50.00,0.00,250.00,'LOT-003','2026-02-07 22:19:54.000000','2026-01-31 22:19:54.000000',NULL),('a1b2c3d4-7000-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000001','a1b2c3d4-0003-0001-0001-000000000001','PROD-001',20.00,20.00,0.00,0.00,4500.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL),('a1b2c3d4-7001-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000001','a1b2c3d4-0003-0001-0001-000000000002','PROD-002',30.00,30.00,0.00,0.00,2800.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL),('a1b2c3d4-7002-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000001','a1b2c3d4-0003-0001-0001-000000000003','PROD-003',50.00,50.00,0.00,0.00,450.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL),('a1b2c3d4-7003-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000002','a1b2c3d4-0003-0001-0001-000000000004','PROD-004',25.00,25.00,25.00,0.00,380.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL),('a1b2c3d4-7004-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000002','a1b2c3d4-0003-0001-0001-000000000005','PROD-005',25.00,25.00,25.00,0.00,5200.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL),('a1b2c3d4-7005-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000003','a1b2c3d4-0003-0001-0001-000000000006','PROD-006',100.00,100.00,100.00,100.00,35.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL),('a1b2c3d4-7006-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000003','a1b2c3d4-0003-0001-0001-000000000007','PROD-007',100.00,100.00,100.00,100.00,320.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL),('a1b2c3d4-7007-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000004','a1b2c3d4-0003-0001-0001-000000000001','PROD-001',5.00,5.00,0.00,0.00,5500.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL),('a1b2c3d4-7008-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000004','a1b2c3d4-0003-0001-0001-000000000005','PROD-005',10.00,10.00,0.00,0.00,6500.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL),('a1b2c3d4-7009-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000005','a1b2c3d4-0003-0001-0001-000000000002','PROD-002',5.00,5.00,5.00,0.00,3500.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL),('a1b2c3d4-7011-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000006','a1b2c3d4-0003-0001-0001-000000000001','PROD-001',10.00,10.00,5.00,0.00,5500.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL),('a1b2c3d4-7012-1111-2222-333333333333','a1b2c3d4-0007-0001-0001-000000000006','a1b2c3d4-0003-0001-0001-000000000002','PROD-002',20.00,20.00,10.00,0.00,3500.00,NULL,NULL,'2026-01-31 22:14:04.000000',NULL);
/*!40000 ALTER TABLE `OrderItems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `OrderPriorityConfigs`
--

DROP TABLE IF EXISTS `OrderPriorityConfigs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `OrderPriorityConfigs` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NamePT` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NameEN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NameES` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DescriptionPT` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DescriptionEN` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DescriptionES` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ColorHex` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SortOrder` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_OrderPriorityConfigs_Code` (`Code`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `OrderPriorityConfigs`
--

LOCK TABLES `OrderPriorityConfigs` WRITE;
/*!40000 ALTER TABLE `OrderPriorityConfigs` DISABLE KEYS */;
INSERT INTO `OrderPriorityConfigs` VALUES (1,'LOW','Baixa','Low','Baja',NULL,NULL,NULL,'#6B7280',0,1,'2025-01-01 00:00:00.000000'),(2,'NORMAL','Normal','Normal','Normal',NULL,NULL,NULL,'#3B82F6',1,1,'2025-01-01 00:00:00.000000'),(3,'HIGH','Alta','High','Alta',NULL,NULL,NULL,'#F59E0B',2,1,'2025-01-01 00:00:00.000000'),(4,'URGENT','Urgente','Urgent','Urgente',NULL,NULL,NULL,'#EF4444',3,1,'2025-01-01 00:00:00.000000');
/*!40000 ALTER TABLE `OrderPriorityConfigs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `OrderStatusConfigs`
--

DROP TABLE IF EXISTS `OrderStatusConfigs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `OrderStatusConfigs` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NamePT` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NameEN` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NameES` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DescriptionPT` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DescriptionEN` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DescriptionES` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ColorHex` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SortOrder` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_OrderStatusConfigs_Code` (`Code`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `OrderStatusConfigs`
--

LOCK TABLES `OrderStatusConfigs` WRITE;
/*!40000 ALTER TABLE `OrderStatusConfigs` DISABLE KEYS */;
INSERT INTO `OrderStatusConfigs` VALUES (1,'DRAFT','Rascunho','Draft','Borrador',NULL,NULL,NULL,'#6B7280',0,1,'2025-01-01 00:00:00.000000'),(2,'PENDING','Pendente','Pending','Pendiente',NULL,NULL,NULL,'#F59E0B',1,1,'2025-01-01 00:00:00.000000'),(3,'CONFIRMED','Confirmado','Confirmed','Confirmado',NULL,NULL,NULL,'#3B82F6',2,1,'2025-01-01 00:00:00.000000'),(4,'IN_PROGRESS','Em Andamento','In Progress','En Progreso',NULL,NULL,NULL,'#8B5CF6',3,1,'2025-01-01 00:00:00.000000'),(5,'PARTIALLY_FULFILLED','Parcialmente Atendido','Partially Fulfilled','Parcialmente Cumplido',NULL,NULL,NULL,'#F59E0B',4,1,'2025-01-01 00:00:00.000000'),(6,'FULFILLED','Atendido','Fulfilled','Cumplido',NULL,NULL,NULL,'#10B981',5,1,'2025-01-01 00:00:00.000000'),(7,'SHIPPED','Enviado','Shipped','Enviado',NULL,NULL,NULL,'#06B6D4',6,1,'2025-01-01 00:00:00.000000'),(8,'DELIVERED','Entregue','Delivered','Entregado',NULL,NULL,NULL,'#22C55E',7,1,'2025-01-01 00:00:00.000000'),(9,'CANCELLED','Cancelado','Cancelled','Cancelado',NULL,NULL,NULL,'#EF4444',8,1,'2025-01-01 00:00:00.000000'),(10,'ON_HOLD','Em Espera','On Hold','En Espera',NULL,NULL,NULL,'#F97316',9,1,'2025-01-01 00:00:00.000000');
/*!40000 ALTER TABLE `OrderStatusConfigs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Orders`
--

DROP TABLE IF EXISTS `Orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Orders` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `OrderNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Source` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CustomerId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `SupplierId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `OrderDate` datetime(6) NOT NULL,
  `ExpectedDate` datetime(6) DEFAULT NULL,
  `Priority` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TotalQuantity` decimal(18,2) NOT NULL,
  `TotalValue` decimal(18,2) NOT NULL,
  `ShippingAddress` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `SpecialInstructions` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsBOPIS` tinyint(1) NOT NULL,
  `CreatedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `ActualDeliveryDate` datetime(6) DEFAULT NULL,
  `DeliveredAt` datetime(6) DEFAULT NULL,
  `DestinationWarehouseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DriverId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `EstimatedDeliveryDate` datetime(6) DEFAULT NULL,
  `OriginWarehouseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `ShippedAt` datetime(6) DEFAULT NULL,
  `ShippingCity` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ShippingCountry` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ShippingLatitude` decimal(10,8) DEFAULT NULL,
  `ShippingLongitude` decimal(11,8) DEFAULT NULL,
  `ShippingState` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ShippingZipCode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `TrackingNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `VehicleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `ActualArrivalPort` datetime(6) DEFAULT NULL,
  `BillOfLading` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CartonsPerParcel` int NOT NULL DEFAULT '0',
  `ContainerNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CustomsBroker` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DesiredMarginPercentage` decimal(5,2) NOT NULL DEFAULT '0.00',
  `DockDoorNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EstimatedArrivalPort` datetime(6) DEFAULT NULL,
  `EstimatedProfit` decimal(18,2) NOT NULL DEFAULT '0.00',
  `ExpectedCartons` int NOT NULL DEFAULT '0',
  `ExpectedParcels` int NOT NULL DEFAULT '0',
  `ImportLicenseNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Incoterm` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsInternational` tinyint(1) NOT NULL DEFAULT '0',
  `IsOwnCarrier` tinyint(1) NOT NULL DEFAULT '0',
  `OriginCountry` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PortOfEntry` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ReceivedParcels` int NOT NULL DEFAULT '0',
  `ShippingCost` decimal(18,2) NOT NULL DEFAULT '0.00',
  `ShippingDistance` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `SuggestedSalePrice` decimal(18,2) NOT NULL DEFAULT '0.00',
  `TaxAmount` decimal(18,2) NOT NULL DEFAULT '0.00',
  `TaxPercentage` decimal(5,2) NOT NULL DEFAULT '0.00',
  `ThirdPartyCarrier` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `TotalCost` decimal(18,2) NOT NULL DEFAULT '0.00',
  `UnitCost` decimal(18,2) NOT NULL DEFAULT '0.00',
  `UnitsPerCarton` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Orders_CompanyId_OrderNumber` (`CompanyId`,`OrderNumber`),
  KEY `IX_Orders_CustomerId` (`CustomerId`),
  KEY `IX_Orders_OrderDate` (`OrderDate`),
  KEY `IX_Orders_Status` (`Status`),
  KEY `IX_Orders_SupplierId` (`SupplierId`),
  KEY `IX_Orders_DestinationWarehouseId` (`DestinationWarehouseId`),
  KEY `IX_Orders_DriverId` (`DriverId`),
  KEY `IX_Orders_OriginWarehouseId` (`OriginWarehouseId`),
  KEY `IX_Orders_TrackingNumber` (`TrackingNumber`),
  KEY `IX_Orders_VehicleId` (`VehicleId`),
  KEY `IX_Orders_ContainerNumber` (`ContainerNumber`),
  KEY `IX_Orders_IsInternational` (`IsInternational`),
  CONSTRAINT `FK_Orders_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Orders_Customers_CustomerId` FOREIGN KEY (`CustomerId`) REFERENCES `Customers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Orders_Drivers_DriverId` FOREIGN KEY (`DriverId`) REFERENCES `Drivers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Orders_Suppliers_SupplierId` FOREIGN KEY (`SupplierId`) REFERENCES `Suppliers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Orders_Vehicles_VehicleId` FOREIGN KEY (`VehicleId`) REFERENCES `Vehicles` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Orders_Warehouses_DestinationWarehouseId` FOREIGN KEY (`DestinationWarehouseId`) REFERENCES `Warehouses` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Orders_Warehouses_OriginWarehouseId` FOREIGN KEY (`OriginWarehouseId`) REFERENCES `Warehouses` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Orders`
--

LOCK TABLES `Orders` WRITE;
/*!40000 ALTER TABLE `Orders` DISABLE KEYS */;
INSERT INTO `Orders` VALUES ('88888888-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','ORD-2026-0001','Sales','Web','33333333-1111-1111-1111-111111111111',NULL,'2026-01-31 22:15:57.000000','2026-02-03 22:15:57.000000','High','Processing',10.00,5500.00,'Rua das Flores, 123','Entregar no horario comercial',0,'ab3c484a-55c9-479c-8b31-ddbc7aed26d4','2026-01-31 22:15:57.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'Sao Paulo','Brasil',NULL,NULL,'SP','01310-100',NULL,NULL,NULL,NULL,1,NULL,NULL,25.00,NULL,NULL,1375.00,2,1,NULL,NULL,0,1,NULL,NULL,0,150.00,NULL,6875.00,990.00,18.00,NULL,4125.00,550.00,5),('88888888-2222-2222-2222-222222222222','a1b2c3d4-5555-1111-2222-333333333333','ORD-2026-0002','Purchase','ERP',NULL,'22222222-1111-1111-1111-111111111111','2026-01-31 22:15:57.000000','2026-02-05 22:15:57.000000','Medium','Pending',50.00,12000.00,'Av. Industrial, 1000','Conferir quantidade na entrega',0,'ab3c484a-55c9-479c-8b31-ddbc7aed26d4','2026-01-31 22:15:57.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'Sao Paulo','Brasil',NULL,NULL,'SP','01310-100',NULL,NULL,NULL,NULL,2,NULL,NULL,30.00,NULL,NULL,3600.00,5,2,NULL,NULL,0,0,NULL,NULL,0,300.00,NULL,15600.00,2160.00,18.00,NULL,8400.00,240.00,10),('88888888-3333-3333-3333-333333333333','a1b2c3d4-5555-1111-2222-333333333333','ORD-2026-0003','Sales','Phone','33333333-2222-2222-2222-222222222222',NULL,'2026-01-31 22:15:57.000000','2026-02-02 22:15:57.000000','High','Shipped',5.00,3200.00,'Av. Brasil, 456','Cliente VIP - prioridade maxima',0,'ab3c484a-55c9-479c-8b31-ddbc7aed26d4','2026-01-31 22:15:57.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'Rio de Janeiro','Brasil',NULL,NULL,'RJ','20040-020',NULL,NULL,NULL,NULL,1,NULL,NULL,20.00,NULL,NULL,640.00,1,1,NULL,NULL,0,1,NULL,NULL,1,80.00,NULL,3840.00,576.00,18.00,NULL,2560.00,640.00,5),('88888888-4444-4444-4444-444444444444','a1b2c3d4-5555-1111-2222-333333333333','ORD-2026-0004','Purchase','ERP',NULL,'22222222-2222-2222-2222-222222222222','2026-01-31 22:15:57.000000','2026-02-07 22:15:57.000000','Low','Approved',100.00,25000.00,'Rua Central, 789','Agendar entrega com antecedencia',0,'ab3c484a-55c9-479c-8b31-ddbc7aed26d4','2026-01-31 22:15:57.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'Belo Horizonte','Brasil',NULL,NULL,'MG','30130-000',NULL,NULL,NULL,NULL,2,NULL,NULL,35.00,NULL,NULL,8750.00,10,4,NULL,NULL,0,0,NULL,NULL,0,500.00,NULL,33750.00,4500.00,18.00,NULL,16250.00,250.00,10),('a1b2c3d4-0007-0001-0001-000000000001','a1b2c3d4-5555-1111-2222-333333333333','ORD-IN-2024-001','Inbound','WMS',NULL,'a1b2c3d4-0001-0001-0001-000000000001','2026-01-31 22:06:32.000000','2026-02-05 22:06:32.000000','High','Pending',100.00,5000.00,NULL,NULL,0,'a1b2c3d4-9999-1111-2222-333333333333','2026-01-31 22:06:32.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,0.00,NULL,NULL,0.00,0,0,NULL,NULL,0,0,NULL,NULL,0,0.00,NULL,0.00,0.00,0.00,NULL,0.00,0.00,0),('a1b2c3d4-0007-0001-0001-000000000002','a1b2c3d4-5555-1111-2222-333333333333','ORD-IN-2024-002','Inbound','WMS',NULL,'a1b2c3d4-0001-0001-0001-000000000002','2026-01-31 22:06:32.000000','2026-02-03 22:06:32.000000','Normal','Processing',50.00,2500.00,NULL,NULL,0,'a1b2c3d4-9999-1111-2222-333333333333','2026-01-31 22:06:32.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,0.00,NULL,NULL,0.00,0,0,NULL,NULL,0,0,NULL,NULL,0,0.00,NULL,0.00,0.00,0.00,NULL,0.00,0.00,0),('a1b2c3d4-0007-0001-0001-000000000003','a1b2c3d4-5555-1111-2222-333333333333','ORD-IN-2024-003','Inbound','WMS',NULL,'a1b2c3d4-0001-0001-0001-000000000003','2026-01-31 22:06:32.000000','2026-02-07 22:06:32.000000','Low','Fulfilled',200.00,10000.00,NULL,NULL,0,'a1b2c3d4-9999-1111-2222-333333333333','2026-01-31 22:06:32.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,0.00,NULL,NULL,0.00,0,0,NULL,NULL,0,0,NULL,NULL,0,0.00,NULL,0.00,0.00,0.00,NULL,0.00,0.00,0),('a1b2c3d4-0007-0001-0001-000000000004','a1b2c3d4-5555-1111-2222-333333333333','ORD-OUT-2024-001','Outbound','WMS','a1b2c3d4-0002-0001-0001-000000000001',NULL,'2026-01-31 22:06:32.000000','2026-02-05 22:06:32.000000','High','Pending',100.00,5000.00,'Av. Paulista, 1000',NULL,0,'a1b2c3d4-9999-1111-2222-333333333333','2026-01-31 22:06:32.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,0.00,NULL,NULL,0.00,0,0,NULL,NULL,0,0,NULL,NULL,0,0.00,NULL,0.00,0.00,0.00,NULL,0.00,0.00,0),('a1b2c3d4-0007-0001-0001-000000000005','a1b2c3d4-5555-1111-2222-333333333333','ORD-OUT-2026-002','Outbound','WMS','a1b2c3d4-0002-0001-0001-000000000002',NULL,'2026-01-31 22:13:37.000000','2026-02-02 22:13:37.000000','Urgent','Confirmed',30.00,18000.00,'Rua do Comércio, 500 - Rio de Janeiro/RJ',NULL,0,'a1b2c3d4-9999-1111-2222-333333333333','2026-01-31 22:13:37.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,0.00,NULL,NULL,0.00,0,0,NULL,NULL,0,0,NULL,NULL,0,0.00,NULL,0.00,0.00,0.00,NULL,0.00,0.00,0),('a1b2c3d4-0007-0001-0001-000000000006','a1b2c3d4-5555-1111-2222-333333333333','ORD-OUT-2026-003','Outbound','WMS','a1b2c3d4-0002-0001-0001-000000000003',NULL,'2026-01-31 22:13:37.000000','2026-02-04 22:13:37.000000','Normal','InProgress',150.00,45000.00,'Av. Brasil, 2000 - Porto Alegre/RS',NULL,0,'a1b2c3d4-9999-1111-2222-333333333333','2026-01-31 22:13:37.000000',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,0.00,NULL,NULL,0.00,0,0,NULL,NULL,0,0,NULL,NULL,0,0.00,NULL,0.00,0.00,0.00,NULL,0.00,0.00,0);
/*!40000 ALTER TABLE `Orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `OutboundShipments`
--

DROP TABLE IF EXISTS `OutboundShipments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `OutboundShipments` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ShipmentNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `OrderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CarrierId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `TrackingNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Status` int NOT NULL,
  `ShippedDate` datetime(6) DEFAULT NULL,
  `DeliveredDate` datetime(6) DEFAULT NULL,
  `DeliveryAddress` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `VehicleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OutboundShipments_OrderId` (`OrderId`),
  KEY `IX_OutboundShipments_VehicleId` (`VehicleId`),
  CONSTRAINT `FK_OutboundShipments_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_OutboundShipments_Vehicles_VehicleId` FOREIGN KEY (`VehicleId`) REFERENCES `Vehicles` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `OutboundShipments`
--

LOCK TABLES `OutboundShipments` WRITE;
/*!40000 ALTER TABLE `OutboundShipments` DISABLE KEYS */;
INSERT INTO `OutboundShipments` VALUES ('c0000001-1111-1111-1111-111111111111','OS-2026-0001','88888888-1111-1111-1111-111111111111','55555555-4444-4444-4444-444444444444','TRK123456789BR',1,'2026-01-31 22:17:56.000000',NULL,'Rua das Flores, 123 - Sao Paulo, SP - 01310-100','2026-01-31 22:17:56.000000',NULL,'55555555-4444-4444-4444-444444444444'),('c0000001-2222-2222-2222-222222222222','OS-2026-0002','88888888-3333-3333-3333-333333333333','55555555-1111-1111-1111-111111111111','TRK789012345BR',2,'2026-01-30 22:17:56.000000','2026-01-31 22:17:56.000000','Av. Brasil, 456 - Rio de Janeiro, RJ - 20040-020','2026-01-31 22:17:56.000000',NULL,'55555555-1111-1111-1111-111111111111'),('c0000001-3333-3333-3333-333333333333','OS-2026-0003','88888888-1111-1111-1111-111111111111','55555555-2222-2222-2222-222222222222','TRK345678901BR',0,NULL,NULL,'Rua Central, 789 - Belo Horizonte, MG - 30130-000','2026-01-31 22:17:56.000000',NULL,'55555555-2222-2222-2222-222222222222');
/*!40000 ALTER TABLE `OutboundShipments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Packages`
--

DROP TABLE IF EXISTS `Packages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Packages` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PackingTaskId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `TrackingNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Type` int NOT NULL,
  `Weight` decimal(65,30) NOT NULL,
  `Length` decimal(65,30) NOT NULL,
  `Width` decimal(65,30) NOT NULL,
  `Height` decimal(65,30) NOT NULL,
  `Status` int NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Packages_PackingTaskId` (`PackingTaskId`),
  CONSTRAINT `FK_Packages_PackingTasks_PackingTaskId` FOREIGN KEY (`PackingTaskId`) REFERENCES `PackingTasks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Packages`
--

LOCK TABLES `Packages` WRITE;
/*!40000 ALTER TABLE `Packages` DISABLE KEYS */;
INSERT INTO `Packages` VALUES ('a39b0437-ff53-11f0-ad5f-52121ab84f7c','a39af3b1-ff53-11f0-ad5f-52121ab84f7c','TRK-0000001',1,1.250000000000000000000000000000,30.000000000000000000000000000000,20.000000000000000000000000000000,10.000000000000000000000000000000,1,'2026-02-01 09:51:54.000000',NULL),('a39b07fb-ff53-11f0-ad5f-52121ab84f7c','a39af3b1-ff53-11f0-ad5f-52121ab84f7c','TRK-0000002',1,0.800000000000000000000000000000,25.000000000000000000000000000000,15.000000000000000000000000000000,8.000000000000000000000000000000,1,'2026-02-01 09:51:54.000000',NULL),('a39b08e4-ff53-11f0-ad5f-52121ab84f7c','a39af485-ff53-11f0-ad5f-52121ab84f7c','TRK-0000003',2,8.500000000000000000000000000000,80.000000000000000000000000000000,60.000000000000000000000000000000,50.000000000000000000000000000000,2,'2026-02-01 09:51:54.000000',NULL);
/*!40000 ALTER TABLE `Packages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PackingTasks`
--

DROP TABLE IF EXISTS `PackingTasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PackingTasks` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `TaskNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `OrderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Status` int NOT NULL,
  `AssignedTo` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompletedAt` datetime(6) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PackingTasks_OrderId` (`OrderId`),
  CONSTRAINT `FK_PackingTasks_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PackingTasks`
--

LOCK TABLES `PackingTasks` WRITE;
/*!40000 ALTER TABLE `PackingTasks` DISABLE KEYS */;
INSERT INTO `PackingTasks` VALUES ('a39af3b1-ff53-11f0-ad5f-52121ab84f7c','PACK-0001','88888888-1111-1111-1111-111111111111',2,'ab3c484a-55c9-479c-8b31-ddbc7aed26d4',NULL,'2026-02-01 09:51:54.000000',NULL),('a39af485-ff53-11f0-ad5f-52121ab84f7c','PACK-0002','88888888-2222-2222-2222-222222222222',3,'ab3c484a-55c9-479c-8b31-ddbc7aed26d4',NULL,'2026-02-01 09:51:54.000000',NULL);
/*!40000 ALTER TABLE `PackingTasks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PickingLines`
--

DROP TABLE IF EXISTS `PickingLines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PickingLines` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PickingTaskId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `OrderItemId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `LocationId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `LotId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `SerialNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `QuantityToPick` decimal(65,30) NOT NULL,
  `QuantityPicked` decimal(65,30) NOT NULL,
  `Status` int NOT NULL,
  `PickedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `PickedAt` datetime(6) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PickingLines_LocationId` (`LocationId`),
  KEY `IX_PickingLines_LotId` (`LotId`),
  KEY `IX_PickingLines_OrderItemId` (`OrderItemId`),
  KEY `IX_PickingLines_PickingTaskId` (`PickingTaskId`),
  KEY `IX_PickingLines_ProductId` (`ProductId`),
  CONSTRAINT `FK_PickingLines_Lots_LotId` FOREIGN KEY (`LotId`) REFERENCES `Lots` (`Id`),
  CONSTRAINT `FK_PickingLines_OrderItems_OrderItemId` FOREIGN KEY (`OrderItemId`) REFERENCES `OrderItems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PickingLines_PickingTasks_PickingTaskId` FOREIGN KEY (`PickingTaskId`) REFERENCES `PickingTasks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PickingLines_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PickingLines_StorageLocations_LocationId` FOREIGN KEY (`LocationId`) REFERENCES `StorageLocations` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PickingLines`
--

LOCK TABLES `PickingLines` WRITE;
/*!40000 ALTER TABLE `PickingLines` DISABLE KEYS */;
INSERT INTO `PickingLines` VALUES ('a38bd845-ff53-11f0-ad5f-52121ab84f7c','a38bc1b6-ff53-11f0-ad5f-52121ab84f7c','20000001-1111-1111-1111-111111111111','77777777-1111-1111-1111-111111111111','a1b2c3d4-1010-1111-2222-333333333333',NULL,NULL,5.000000000000000000000000000000,0.000000000000000000000000000000,0,NULL,NULL,'2026-02-01 09:51:53.000000',NULL),('a38be2d8-ff53-11f0-ad5f-52121ab84f7c','a38bc36f-ff53-11f0-ad5f-52121ab84f7c','20000001-2222-2222-2222-222222222222','77777777-2222-2222-2222-222222222222','e0000001-1111-1111-1111-111111111111',NULL,NULL,25.000000000000000000000000000000,0.000000000000000000000000000000,0,NULL,NULL,'2026-02-01 09:51:53.000000',NULL),('a38be4da-ff53-11f0-ad5f-52121ab84f7c','a38bc4e3-ff53-11f0-ad5f-52121ab84f7c','20000001-3333-3333-3333-333333333333','77777777-5555-5555-5555-555555555555','e0000001-2222-2222-2222-222222222222',NULL,NULL,5.000000000000000000000000000000,1.000000000000000000000000000000,2,NULL,NULL,'2026-02-01 09:51:53.000000',NULL);
/*!40000 ALTER TABLE `PickingLines` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PickingTasks`
--

DROP TABLE IF EXISTS `PickingTasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PickingTasks` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `TaskNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PickingWaveId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `OrderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Priority` int NOT NULL,
  `Status` int NOT NULL,
  `AssignedTo` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `CompletedAt` datetime(6) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PickingTasks_OrderId` (`OrderId`),
  KEY `IX_PickingTasks_PickingWaveId` (`PickingWaveId`),
  CONSTRAINT `FK_PickingTasks_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PickingTasks_PickingWaves_PickingWaveId` FOREIGN KEY (`PickingWaveId`) REFERENCES `PickingWaves` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PickingTasks`
--

LOCK TABLES `PickingTasks` WRITE;
/*!40000 ALTER TABLE `PickingTasks` DISABLE KEYS */;
INSERT INTO `PickingTasks` VALUES ('a38bc1b6-ff53-11f0-ad5f-52121ab84f7c','PICK-0001','a38bb175-ff53-11f0-ad5f-52121ab84f7c','88888888-1111-1111-1111-111111111111',2,1,NULL,NULL,'2026-02-01 09:51:53.000000',NULL),('a38bc36f-ff53-11f0-ad5f-52121ab84f7c','PICK-0002','a38bb175-ff53-11f0-ad5f-52121ab84f7c','88888888-2222-2222-2222-222222222222',3,2,'ab3c484a-55c9-479c-8b31-ddbc7aed26d4',NULL,'2026-02-01 09:51:53.000000',NULL),('a38bc4e3-ff53-11f0-ad5f-52121ab84f7c','PICK-0003','a38bb175-ff53-11f0-ad5f-52121ab84f7c','88888888-3333-3333-3333-333333333333',4,3,'ab3c484a-55c9-479c-8b31-ddbc7aed26d4',NULL,'2026-02-01 09:51:53.000000',NULL);
/*!40000 ALTER TABLE `PickingTasks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PickingWaves`
--

DROP TABLE IF EXISTS `PickingWaves`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PickingWaves` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `WaveNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `WarehouseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Status` int NOT NULL,
  `ReleasedAt` datetime(6) DEFAULT NULL,
  `CompletedAt` datetime(6) DEFAULT NULL,
  `TotalOrders` int NOT NULL,
  `TotalLines` int NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PickingWaves_WarehouseId` (`WarehouseId`),
  CONSTRAINT `FK_PickingWaves_Warehouses_WarehouseId` FOREIGN KEY (`WarehouseId`) REFERENCES `Warehouses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PickingWaves`
--

LOCK TABLES `PickingWaves` WRITE;
/*!40000 ALTER TABLE `PickingWaves` DISABLE KEYS */;
INSERT INTO `PickingWaves` VALUES ('a38bb175-ff53-11f0-ad5f-52121ab84f7c','WAVE-20260201-01','a1b2c3d4-1000-1111-2222-333333333333',1,NULL,NULL,3,3,'2026-02-01 09:51:53.000000',NULL);
/*!40000 ALTER TABLE `PickingWaves` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ProductCategories`
--

DROP TABLE IF EXISTS `ProductCategories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ProductCategories` (
  `Id` char(36) NOT NULL,
  `CompanyId` char(36) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `ParentId` char(36) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `Attributes` text,
  `Reference` varchar(100) DEFAULT NULL,
  `IsMaintenance` tinyint(1) NOT NULL DEFAULT '0',
  `Code` varchar(50) DEFAULT NULL,
  `Barcode` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ProductCategories`
--

LOCK TABLES `ProductCategories` WRITE;
/*!40000 ALTER TABLE `ProductCategories` DISABLE KEYS */;
INSERT INTO `ProductCategories` VALUES ('66666666-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','Eletrônicos','Produtos eletrônicos em geral',NULL,1,'2026-01-31 22:15:02.000000',NULL,NULL,NULL,0,'CAT-66666666',NULL),('66666666-2222-2222-2222-222222222222','a1b2c3d4-5555-1111-2222-333333333333','Alimentos','Produtos alimentícios',NULL,1,'2026-01-31 22:15:02.000000',NULL,NULL,NULL,0,'CAT-66666666',NULL),('66666666-3333-3333-3333-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Bebidas','Bebidas em geral',NULL,1,'2026-01-31 22:15:02.000000',NULL,NULL,NULL,0,'CAT-66666666',NULL),('66666666-4444-4444-4444-444444444444','a1b2c3d4-5555-1111-2222-333333333333','Vestuário','Roupas e acessórios',NULL,1,'2026-01-31 22:15:02.000000',NULL,NULL,NULL,0,'CAT-66666666',NULL),('66666666-5555-5555-5555-555555555555','a1b2c3d4-5555-1111-2222-333333333333','Móveis','Móveis e decoração',NULL,1,'2026-01-31 22:15:02.000000',NULL,NULL,NULL,0,'CAT-66666666',NULL),('a1b2c3d4-0000-0001-0001-000000000001','a1b2c3d4-5555-1111-2222-333333333333','Eletrônicos','Produtos eletrônicos',NULL,1,'2026-01-31 22:04:59.000000',NULL,NULL,NULL,0,'CAT-a1b2c3d4',NULL),('a1b2c3d4-0000-0001-0001-000000000002','a1b2c3d4-5555-1111-2222-333333333333','Informática','Equipamentos de informática',NULL,1,'2026-01-31 22:04:59.000000',NULL,NULL,NULL,0,'CAT-a1b2c3d4',NULL),('a1b2c3d4-0000-0001-0001-000000000003','a1b2c3d4-5555-1111-2222-333333333333','Periféricos','Periféricos de computador',NULL,1,'2026-01-31 22:04:59.000000',NULL,NULL,NULL,0,'CAT-a1b2c3d4',NULL),('a1b2c3d4-0000-0001-0001-000000000004','a1b2c3d4-5555-1111-2222-333333333333','Acessórios','Acessórios diversos',NULL,1,'2026-01-31 22:04:59.000000',NULL,NULL,NULL,0,'CAT-a1b2c3d4',NULL),('a1b2c3d4-0000-0001-0001-000000000005','a1b2c3d4-5555-1111-2222-333333333333','Cabos','Cabos e conectores',NULL,1,'2026-01-31 22:04:59.000000',NULL,NULL,NULL,0,'CAT-a1b2c3d4',NULL);
/*!40000 ALTER TABLE `ProductCategories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Products`
--

DROP TABLE IF EXISTS `Products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Products` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SKU` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Barcode` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Weight` decimal(65,30) NOT NULL,
  `WeightUnit` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Volume` decimal(65,30) NOT NULL,
  `VolumeUnit` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Length` decimal(65,30) NOT NULL,
  `Width` decimal(65,30) NOT NULL,
  `Height` decimal(65,30) NOT NULL,
  `DimensionUnit` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `RequiresLotTracking` tinyint(1) NOT NULL,
  `RequiresSerialTracking` tinyint(1) NOT NULL,
  `IsPerishable` tinyint(1) NOT NULL,
  `ShelfLifeDays` int DEFAULT NULL,
  `MinimumStock` decimal(65,30) DEFAULT NULL,
  `SafetyStock` decimal(65,30) DEFAULT NULL,
  `ABCClassification` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `CategoryId` char(36) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Products_SKU` (`SKU`),
  UNIQUE KEY `IX_Products_Barcode` (`Barcode`),
  KEY `IX_Products_CompanyId` (`CompanyId`),
  CONSTRAINT `FK_Products_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Products`
--

LOCK TABLES `Products` WRITE;
/*!40000 ALTER TABLE `Products` DISABLE KEYS */;
INSERT INTO `Products` VALUES ('77777777-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','Smartphone Galaxy S24','SKU-001','7891234560001','Smartphone Samsung Galaxy S24 Ultra 256GB',0.234000000000000000000000000000,'kg',0.000250000000000000000000000000,'m3',7.700000000000000000000000000000,16.200000000000000000000000000000,0.860000000000000000000000000000,'cm',0,1,0,NULL,50.000000000000000000000000000000,20.000000000000000000000000000000,'A',1,'2026-01-31 22:15:02.000000',NULL,'66666666-1111-1111-1111-111111111111'),('77777777-2222-2222-2222-222222222222','a1b2c3d4-5555-1111-2222-333333333333','Notebook Dell XPS 15','SKU-002','7891234560002','Notebook Dell XPS 15 i7 32GB RAM 1TB SSD',1.860000000000000000000000000000,'kg',0.003000000000000000000000000000,'m3',34.400000000000000000000000000000,23.000000000000000000000000000000,1.800000000000000000000000000000,'cm',0,1,0,NULL,20.000000000000000000000000000000,10.000000000000000000000000000000,'A',1,'2026-01-31 22:15:02.000000',NULL,'66666666-1111-1111-1111-111111111111'),('77777777-3333-3333-3333-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Arroz Tipo 1 5kg','SKU-003','7891234560003','Arroz branco tipo 1 pacote 5kg',5.100000000000000000000000000000,'kg',0.007000000000000000000000000000,'m3',30.000000000000000000000000000000,20.000000000000000000000000000000,12.000000000000000000000000000000,'cm',1,0,0,365,100.000000000000000000000000000000,50.000000000000000000000000000000,'A',1,'2026-01-31 22:15:02.000000',NULL,'66666666-2222-2222-2222-222222222222'),('77777777-4444-4444-4444-444444444444','a1b2c3d4-5555-1111-2222-333333333333','Agua Mineral 500ml','SKU-004','7891234560004','Agua mineral sem gas 500ml',0.520000000000000000000000000000,'kg',0.000500000000000000000000000000,'m3',6.000000000000000000000000000000,6.000000000000000000000000000000,22.000000000000000000000000000000,'cm',1,0,0,180,500.000000000000000000000000000000,200.000000000000000000000000000000,'B',1,'2026-01-31 22:15:02.000000',NULL,'66666666-3333-3333-3333-333333333333'),('77777777-5555-5555-5555-555555555555','a1b2c3d4-5555-1111-2222-333333333333','Camiseta Basica M','SKU-005','7891234560005','Camiseta basica algodao tamanho M',0.200000000000000000000000000000,'kg',0.001000000000000000000000000000,'m3',30.000000000000000000000000000000,40.000000000000000000000000000000,2.000000000000000000000000000000,'cm',0,0,0,NULL,200.000000000000000000000000000000,80.000000000000000000000000000000,'B',1,'2026-01-31 22:15:02.000000',NULL,'66666666-4444-4444-4444-444444444444'),('a1b2c3d4-0003-0001-0001-000000000001','a1b2c3d4-5555-1111-2222-333333333333','Notebook Dell Latitude','PROD-001','7891234567890','Notebook Dell i7 16GB',2.500000000000000000000000000000,NULL,0.010000000000000000000000000000,NULL,35.000000000000000000000000000000,25.000000000000000000000000000000,2.000000000000000000000000000000,NULL,0,1,0,NULL,5.000000000000000000000000000000,10.000000000000000000000000000000,NULL,1,'2026-01-31 22:05:13.000000',NULL,'a1b2c3d4-0000-0001-0001-000000000002'),('a1b2c3d4-0003-0001-0001-000000000002','a1b2c3d4-5555-1111-2222-333333333333','Monitor LG 27pol','PROD-002','7891234567891','Monitor LG UltraWide',5.000000000000000000000000000000,NULL,0.050000000000000000000000000000,NULL,65.000000000000000000000000000000,45.000000000000000000000000000000,15.000000000000000000000000000000,NULL,0,0,0,NULL,3.000000000000000000000000000000,5.000000000000000000000000000000,NULL,1,'2026-01-31 22:05:13.000000',NULL,'a1b2c3d4-0000-0001-0001-000000000001'),('a1b2c3d4-0003-0001-0001-000000000003','a1b2c3d4-5555-1111-2222-333333333333','Teclado Logitech MX','PROD-003','7891234567892','Teclado mecânico wireless',0.800000000000000000000000000000,NULL,0.002000000000000000000000000000,NULL,45.000000000000000000000000000000,15.000000000000000000000000000000,3.000000000000000000000000000000,NULL,0,0,0,NULL,10.000000000000000000000000000000,15.000000000000000000000000000000,NULL,1,'2026-01-31 22:05:13.000000',NULL,'a1b2c3d4-0000-0001-0001-000000000003'),('a1b2c3d4-0003-0001-0001-000000000004','a1b2c3d4-5555-1111-2222-333333333333','Mouse Logitech MX','PROD-004','7891234567893','Mouse wireless ergonômico',0.150000000000000000000000000000,NULL,0.000500000000000000000000000000,NULL,12.000000000000000000000000000000,7.000000000000000000000000000000,4.000000000000000000000000000000,NULL,0,0,0,NULL,10.000000000000000000000000000000,20.000000000000000000000000000000,NULL,1,'2026-01-31 22:05:13.000000',NULL,'a1b2c3d4-0000-0001-0001-000000000003'),('a1b2c3d4-0003-0001-0001-000000000005','a1b2c3d4-5555-1111-2222-333333333333','Cabo HDMI 2m','PROD-005','7891234567894','Cabo HDMI 2.1 4K',0.100000000000000000000000000000,NULL,0.000100000000000000000000000000,NULL,200.000000000000000000000000000000,2.000000000000000000000000000000,2.000000000000000000000000000000,NULL,0,0,0,NULL,20.000000000000000000000000000000,30.000000000000000000000000000000,NULL,1,'2026-01-31 22:05:13.000000',NULL,'a1b2c3d4-0000-0001-0001-000000000005'),('a1b2c3d4-0003-0001-0001-000000000006','a1b2c3d4-5555-1111-2222-333333333333','Cabo HDMI 2.1 2m','PROD-006','7891234567895','Cabo HDMI 2.1 8K 48Gbps',0.100000000000000000000000000000,NULL,0.000100000000000000000000000000,NULL,200.000000000000000000000000000000,2.000000000000000000000000000000,2.000000000000000000000000000000,NULL,0,0,0,NULL,20.000000000000000000000000000000,30.000000000000000000000000000000,NULL,1,'2026-01-31 22:11:40.000000',NULL,'a1b2c3d4-0000-0001-0001-000000000004'),('a1b2c3d4-0003-0001-0001-000000000007','a1b2c3d4-5555-1111-2222-333333333333','Webcam Logitech C920','PROD-007','7891234567896','Webcam Full HD 1080p',0.200000000000000000000000000000,NULL,0.000400000000000000000000000000,NULL,10.000000000000000000000000000000,8.000000000000000000000000000000,5.000000000000000000000000000000,NULL,0,0,0,NULL,8.000000000000000000000000000000,12.000000000000000000000000000000,NULL,1,'2026-01-31 22:11:40.000000',NULL,'a1b2c3d4-0000-0001-0001-000000000003'),('a1b2c3d4-0003-0001-0001-000000000008','a1b2c3d4-5555-1111-2222-333333333333','Headset HyperX Cloud II','PROD-008','7891234567897','Headset gamer 7.1 surround',0.350000000000000000000000000000,NULL,0.003000000000000000000000000000,NULL,20.000000000000000000000000000000,18.000000000000000000000000000000,10.000000000000000000000000000000,NULL,0,0,0,NULL,6.000000000000000000000000000000,10.000000000000000000000000000000,NULL,1,'2026-01-31 22:11:40.000000',NULL,'a1b2c3d4-0000-0001-0001-000000000003');
/*!40000 ALTER TABLE `Products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PurchaseOrderDocuments`
--

DROP TABLE IF EXISTS `PurchaseOrderDocuments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PurchaseOrderDocuments` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PurchaseOrderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FileName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FilePath` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FileUrl` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FileSizeBytes` bigint NOT NULL,
  `MimeType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UploadedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UploadedAt` datetime(6) NOT NULL,
  `DeletedAt` datetime(6) DEFAULT NULL,
  `DeletedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PurchaseOrderDocuments_PurchaseOrderId` (`PurchaseOrderId`),
  KEY `IX_PurchaseOrderDocuments_Type` (`Type`),
  KEY `IX_PurchaseOrderDocuments_UploadedAt` (`UploadedAt`),
  CONSTRAINT `FK_PurchaseOrderDocuments_PurchaseOrders_PurchaseOrderId` FOREIGN KEY (`PurchaseOrderId`) REFERENCES `PurchaseOrders` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PurchaseOrderDocuments`
--

LOCK TABLES `PurchaseOrderDocuments` WRITE;
/*!40000 ALTER TABLE `PurchaseOrderDocuments` DISABLE KEYS */;
/*!40000 ALTER TABLE `PurchaseOrderDocuments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PurchaseOrderItems`
--

DROP TABLE IF EXISTS `PurchaseOrderItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PurchaseOrderItems` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PurchaseOrderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SKU` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `QuantityOrdered` decimal(18,2) NOT NULL,
  `QuantityReceived` decimal(18,2) NOT NULL,
  `UnitPrice` decimal(18,2) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PurchaseOrderItems_ProductId` (`ProductId`),
  KEY `IX_PurchaseOrderItems_PurchaseOrderId` (`PurchaseOrderId`),
  KEY `IX_PurchaseOrderItems_SKU` (`SKU`),
  CONSTRAINT `FK_PurchaseOrderItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId` FOREIGN KEY (`PurchaseOrderId`) REFERENCES `PurchaseOrders` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PurchaseOrderItems`
--

LOCK TABLES `PurchaseOrderItems` WRITE;
/*!40000 ALTER TABLE `PurchaseOrderItems` DISABLE KEYS */;
INSERT INTO `PurchaseOrderItems` VALUES ('30000001-1111-1111-1111-111111111111','99999999-1111-1111-1111-111111111111','77777777-1111-1111-1111-111111111111','SKU-001',50.00,0.00,150.00),('30000001-2222-2222-2222-222222222222','99999999-1111-1111-1111-111111111111','77777777-2222-2222-2222-222222222222','SKU-002',50.00,0.00,150.00),('30000001-3333-3333-3333-333333333333','99999999-2222-2222-2222-222222222222','77777777-3333-3333-3333-333333333333','SKU-003',50.00,25.00,170.00),('30000001-4444-4444-4444-444444444444','99999999-3333-3333-3333-333333333333','77777777-4444-4444-4444-444444444444','SKU-004',100.00,50.00,160.00),('30000001-5555-5555-5555-555555555555','99999999-3333-3333-3333-333333333333','77777777-5555-5555-5555-555555555555','SKU-005',100.00,50.00,160.00);
/*!40000 ALTER TABLE `PurchaseOrderItems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PurchaseOrders`
--

DROP TABLE IF EXISTS `PurchaseOrders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PurchaseOrders` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PurchaseOrderNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SupplierId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `OrderDate` datetime(6) NOT NULL,
  `ExpectedDate` datetime(6) DEFAULT NULL,
  `Priority` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TotalQuantity` decimal(18,2) NOT NULL,
  `TotalValue` decimal(18,2) NOT NULL,
  `SpecialInstructions` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UnitCost` decimal(18,2) NOT NULL,
  `TotalCost` decimal(18,2) NOT NULL,
  `TaxAmount` decimal(18,2) NOT NULL,
  `TaxPercentage` decimal(5,2) NOT NULL,
  `DesiredMarginPercentage` decimal(5,2) NOT NULL,
  `SuggestedSalePrice` decimal(18,2) NOT NULL,
  `EstimatedProfit` decimal(18,2) NOT NULL,
  `ExpectedParcels` int NOT NULL,
  `ReceivedParcels` int NOT NULL,
  `ExpectedCartons` int NOT NULL,
  `UnitsPerCarton` int NOT NULL,
  `CartonsPerParcel` int NOT NULL,
  `ShippingDistance` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ShippingCost` decimal(18,2) NOT NULL,
  `DestinationWarehouseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `VehicleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DriverId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DockDoorNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsInternational` tinyint(1) NOT NULL,
  `OriginCountry` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PortOfEntry` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CustomsBroker` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsOwnCarrier` tinyint(1) NOT NULL,
  `ThirdPartyCarrier` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ContainerNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `BillOfLading` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ImportLicenseNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `EstimatedArrivalPort` datetime(6) DEFAULT NULL,
  `ActualArrivalPort` datetime(6) DEFAULT NULL,
  `Incoterm` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_PurchaseOrders_CompanyId_PurchaseOrderNumber` (`CompanyId`,`PurchaseOrderNumber`),
  KEY `IX_PurchaseOrders_OrderDate` (`OrderDate`),
  KEY `IX_PurchaseOrders_Status` (`Status`),
  KEY `IX_PurchaseOrders_SupplierId` (`SupplierId`),
  CONSTRAINT `FK_PurchaseOrders_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PurchaseOrders_Suppliers_SupplierId` FOREIGN KEY (`SupplierId`) REFERENCES `Suppliers` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PurchaseOrders`
--

LOCK TABLES `PurchaseOrders` WRITE;
/*!40000 ALTER TABLE `PurchaseOrders` DISABLE KEYS */;
INSERT INTO `PurchaseOrders` VALUES ('99999999-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','PO-2026-0001','22222222-1111-1111-1111-111111111111','2026-01-31 22:16:46.000000','2026-02-07 22:16:46.000000','High','Pending',100.00,15000.00,'Verificar qualidade na entrega',150.00,15000.00,2700.00,18.00,30.00,19500.00,4500.00,2,0,4,25,2,'50km',250.00,'11111111-1111-1111-1111-111111111111','55555555-1111-1111-1111-111111111111','44444444-1111-1111-1111-111111111111',NULL,0,NULL,NULL,NULL,1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2026-01-31 22:16:46.000000',NULL),('99999999-2222-2222-2222-222222222222','a1b2c3d4-5555-1111-2222-333333333333','PO-2026-0002','22222222-2222-2222-2222-222222222222','2026-01-31 22:16:46.000000','2026-02-05 22:16:46.000000','Medium','Approved',50.00,8500.00,'Entregar pela manha',170.00,8500.00,1530.00,18.00,25.00,10625.00,2125.00,1,0,2,25,2,'30km',120.00,'11111111-1111-1111-1111-111111111111','55555555-2222-2222-2222-222222222222','44444444-2222-2222-2222-222222222222',NULL,0,NULL,NULL,NULL,1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2026-01-31 22:16:46.000000',NULL),('99999999-3333-3333-3333-333333333333','a1b2c3d4-5555-1111-2222-333333333333','PO-2026-0003','22222222-3333-3333-3333-333333333333','2026-01-31 22:16:46.000000','2026-02-10 22:16:46.000000','Low','Processing',200.00,32000.00,'Descarregar na doca 2',160.00,32000.00,5760.00,18.00,35.00,43200.00,11200.00,3,1,8,25,3,'80km',400.00,'11111111-1111-1111-1111-111111111111','55555555-3333-3333-3333-333333333333','44444444-3333-3333-3333-333333333333',NULL,0,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2026-01-31 22:16:46.000000',NULL);
/*!40000 ALTER TABLE `PurchaseOrders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PutawayTasks`
--

DROP TABLE IF EXISTS `PutawayTasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PutawayTasks` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `TaskNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ReceiptId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `LotId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `Quantity` decimal(65,30) NOT NULL,
  `FromLocationId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ToLocationId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Priority` int NOT NULL,
  `Status` int NOT NULL,
  `AssignedTo` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `CompletedAt` datetime(6) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PutawayTasks_FromLocationId` (`FromLocationId`),
  KEY `IX_PutawayTasks_LotId` (`LotId`),
  KEY `IX_PutawayTasks_ProductId` (`ProductId`),
  KEY `IX_PutawayTasks_ReceiptId` (`ReceiptId`),
  KEY `IX_PutawayTasks_ToLocationId` (`ToLocationId`),
  CONSTRAINT `FK_PutawayTasks_Lots_LotId` FOREIGN KEY (`LotId`) REFERENCES `Lots` (`Id`),
  CONSTRAINT `FK_PutawayTasks_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PutawayTasks_Receipts_ReceiptId` FOREIGN KEY (`ReceiptId`) REFERENCES `Receipts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PutawayTasks_StorageLocations_FromLocationId` FOREIGN KEY (`FromLocationId`) REFERENCES `StorageLocations` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PutawayTasks_StorageLocations_ToLocationId` FOREIGN KEY (`ToLocationId`) REFERENCES `StorageLocations` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PutawayTasks`
--

LOCK TABLES `PutawayTasks` WRITE;
/*!40000 ALTER TABLE `PutawayTasks` DISABLE KEYS */;
/*!40000 ALTER TABLE `PutawayTasks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ReceiptLines`
--

DROP TABLE IF EXISTS `ReceiptLines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ReceiptLines` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ReceiptId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SKU` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LotNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SerialNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `QuantityExpected` decimal(65,30) NOT NULL,
  `QuantityReceived` decimal(65,30) NOT NULL,
  `QuantityDamaged` decimal(65,30) NOT NULL,
  `InspectionStatus` int NOT NULL,
  `QualityNotes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ExpiryDate` datetime(6) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ReceiptLines_ProductId` (`ProductId`),
  KEY `IX_ReceiptLines_ReceiptId` (`ReceiptId`),
  CONSTRAINT `FK_ReceiptLines_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ReceiptLines_Receipts_ReceiptId` FOREIGN KEY (`ReceiptId`) REFERENCES `Receipts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ReceiptLines`
--

LOCK TABLES `ReceiptLines` WRITE;
/*!40000 ALTER TABLE `ReceiptLines` DISABLE KEYS */;
/*!40000 ALTER TABLE `ReceiptLines` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Receipts`
--

DROP TABLE IF EXISTS `Receipts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Receipts` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ReceiptNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `InboundShipmentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ReceiptDate` datetime(6) NOT NULL,
  `Status` int NOT NULL,
  `WarehouseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ReceivedBy` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Receipts_InboundShipmentId` (`InboundShipmentId`),
  KEY `IX_Receipts_WarehouseId` (`WarehouseId`),
  CONSTRAINT `FK_Receipts_InboundShipments_InboundShipmentId` FOREIGN KEY (`InboundShipmentId`) REFERENCES `InboundShipments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Receipts_Warehouses_WarehouseId` FOREIGN KEY (`WarehouseId`) REFERENCES `Warehouses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Receipts`
--

LOCK TABLES `Receipts` WRITE;
/*!40000 ALTER TABLE `Receipts` DISABLE KEYS */;
/*!40000 ALTER TABLE `Receipts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SalesOrderItems`
--

DROP TABLE IF EXISTS `SalesOrderItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SalesOrderItems` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SalesOrderId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SKU` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `QuantityOrdered` decimal(18,2) NOT NULL,
  `QuantityAllocated` decimal(18,2) NOT NULL,
  `QuantityPicked` decimal(18,2) NOT NULL,
  `QuantityShipped` decimal(18,2) NOT NULL,
  `UnitPrice` decimal(18,2) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_SalesOrderItems_ProductId` (`ProductId`),
  KEY `IX_SalesOrderItems_SalesOrderId` (`SalesOrderId`),
  KEY `IX_SalesOrderItems_SKU` (`SKU`),
  CONSTRAINT `FK_SalesOrderItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_SalesOrderItems_SalesOrders_SalesOrderId` FOREIGN KEY (`SalesOrderId`) REFERENCES `SalesOrders` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SalesOrderItems`
--

LOCK TABLES `SalesOrderItems` WRITE;
/*!40000 ALTER TABLE `SalesOrderItems` DISABLE KEYS */;
INSERT INTO `SalesOrderItems` VALUES ('40000001-1111-1111-1111-111111111111','a0000001-1111-1111-1111-111111111111','77777777-1111-1111-1111-111111111111','SKU-001',5.00,5.00,3.00,0.00,2500.00),('40000001-2222-2222-2222-222222222222','a0000001-2222-2222-2222-222222222222','77777777-2222-2222-2222-222222222222','SKU-002',5.00,5.00,4.00,2.00,890.00),('40000001-3333-3333-3333-333333333333','a0000001-2222-2222-2222-222222222222','77777777-5555-5555-5555-555555555555','SKU-005',5.00,5.00,5.00,3.00,800.00),('40000001-4444-4444-4444-444444444444','a0000001-3333-3333-3333-333333333333','77777777-3333-3333-3333-333333333333','SKU-003',3.00,3.00,3.00,3.00,1500.00);
/*!40000 ALTER TABLE `SalesOrderItems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SalesOrders`
--

DROP TABLE IF EXISTS `SalesOrders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SalesOrders` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SalesOrderNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CustomerId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `OrderDate` datetime(6) NOT NULL,
  `ExpectedDate` datetime(6) DEFAULT NULL,
  `Priority` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TotalQuantity` decimal(18,2) NOT NULL,
  `TotalValue` decimal(18,2) NOT NULL,
  `SpecialInstructions` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ShippingAddress` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ShippingZipCode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ShippingLatitude` decimal(10,7) DEFAULT NULL,
  `ShippingLongitude` decimal(10,7) DEFAULT NULL,
  `ShippingCity` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ShippingState` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ShippingCountry` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `TrackingNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EstimatedDeliveryDate` datetime(6) DEFAULT NULL,
  `ActualDeliveryDate` datetime(6) DEFAULT NULL,
  `ShippedAt` datetime(6) DEFAULT NULL,
  `DeliveredAt` datetime(6) DEFAULT NULL,
  `IsBOPIS` tinyint(1) NOT NULL,
  `ExpectedParcels` int NOT NULL,
  `PackedParcels` int NOT NULL,
  `ExpectedCartons` int NOT NULL,
  `UnitsPerCarton` int NOT NULL,
  `CartonsPerParcel` int NOT NULL,
  `VehicleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DriverId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `OriginWarehouseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_SalesOrders_CompanyId_SalesOrderNumber` (`CompanyId`,`SalesOrderNumber`),
  KEY `IX_SalesOrders_CustomerId` (`CustomerId`),
  KEY `IX_SalesOrders_OrderDate` (`OrderDate`),
  KEY `IX_SalesOrders_Status` (`Status`),
  KEY `IX_SalesOrders_TrackingNumber` (`TrackingNumber`),
  CONSTRAINT `FK_SalesOrders_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_SalesOrders_Customers_CustomerId` FOREIGN KEY (`CustomerId`) REFERENCES `Customers` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SalesOrders`
--

LOCK TABLES `SalesOrders` WRITE;
/*!40000 ALTER TABLE `SalesOrders` DISABLE KEYS */;
INSERT INTO `SalesOrders` VALUES ('a0000001-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','SO-2026-0001','33333333-1111-1111-1111-111111111111','2026-01-31 22:17:13.000000','2026-02-03 22:17:13.000000','High','Pending',5.00,12500.00,'Cliente VIP - entregar com cuidado','Rua das Flores, 123','01310-100',NULL,NULL,'Sao Paulo','SP','Brasil',NULL,NULL,NULL,NULL,NULL,0,1,0,1,5,1,'55555555-1111-1111-1111-111111111111','44444444-1111-1111-1111-111111111111','11111111-1111-1111-1111-111111111111','2026-01-31 22:17:13.000000',NULL),('a0000001-2222-2222-2222-222222222222','a1b2c3d4-5555-1111-2222-333333333333','SO-2026-0002','33333333-2222-2222-2222-222222222222','2026-01-31 22:17:13.000000','2026-02-05 22:17:13.000000','Medium','Processing',10.00,8900.00,'Agendar entrega com cliente','Av. Brasil, 456','20040-020',NULL,NULL,'Rio de Janeiro','RJ','Brasil',NULL,NULL,NULL,NULL,NULL,0,2,1,2,5,1,'55555555-2222-2222-2222-222222222222','44444444-2222-2222-2222-222222222222','11111111-1111-1111-1111-111111111111','2026-01-31 22:17:13.000000',NULL),('a0000001-3333-3333-3333-333333333333','a1b2c3d4-5555-1111-2222-333333333333','SO-2026-0003','33333333-3333-3333-3333-333333333333','2026-01-31 22:17:13.000000','2026-02-02 22:17:13.000000','High','Shipped',3.00,4500.00,'Urgente - entrega expressa','Rua Central, 789','30130-000',NULL,NULL,'Belo Horizonte','MG','Brasil',NULL,NULL,NULL,NULL,NULL,0,1,1,1,3,1,'55555555-3333-3333-3333-333333333333','44444444-3333-3333-3333-333333333333','11111111-1111-1111-1111-111111111111','2026-01-31 22:17:13.000000',NULL);
/*!40000 ALTER TABLE `SalesOrders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SerialNumbers`
--

DROP TABLE IF EXISTS `SerialNumbers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SerialNumbers` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Serial` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `LotId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Status` int NOT NULL,
  `CurrentLocationId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `ReceivedDate` datetime(6) DEFAULT NULL,
  `ShippedDate` datetime(6) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_SerialNumbers_CurrentLocationId` (`CurrentLocationId`),
  KEY `IX_SerialNumbers_LotId` (`LotId`),
  KEY `IX_SerialNumbers_ProductId` (`ProductId`),
  CONSTRAINT `FK_SerialNumbers_Lots_LotId` FOREIGN KEY (`LotId`) REFERENCES `Lots` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_SerialNumbers_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_SerialNumbers_StorageLocations_CurrentLocationId` FOREIGN KEY (`CurrentLocationId`) REFERENCES `StorageLocations` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SerialNumbers`
--

LOCK TABLES `SerialNumbers` WRITE;
/*!40000 ALTER TABLE `SerialNumbers` DISABLE KEYS */;
/*!40000 ALTER TABLE `SerialNumbers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `StockMovements`
--

DROP TABLE IF EXISTS `StockMovements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `StockMovements` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ProductId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `StorageLocationId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Type` int NOT NULL,
  `Quantity` decimal(65,30) NOT NULL,
  `Reference` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Notes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MovementDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_StockMovements_ProductId` (`ProductId`),
  KEY `IX_StockMovements_StorageLocationId` (`StorageLocationId`),
  CONSTRAINT `FK_StockMovements_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_StockMovements_StorageLocations_StorageLocationId` FOREIGN KEY (`StorageLocationId`) REFERENCES `StorageLocations` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `StockMovements`
--

LOCK TABLES `StockMovements` WRITE;
/*!40000 ALTER TABLE `StockMovements` DISABLE KEYS */;
/*!40000 ALTER TABLE `StockMovements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `StorageLocations`
--

DROP TABLE IF EXISTS `StorageLocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `StorageLocations` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `WarehouseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ZoneId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `Code` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Aisle` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Rack` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Level` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Position` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Type` int NOT NULL,
  `MaxWeight` decimal(65,30) NOT NULL,
  `MaxVolume` decimal(65,30) NOT NULL,
  `CurrentWeight` decimal(65,30) NOT NULL,
  `CurrentVolume` decimal(65,30) NOT NULL,
  `IsBlocked` tinyint(1) NOT NULL,
  `BlockReason` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_StorageLocations_WarehouseId` (`WarehouseId`),
  KEY `IX_StorageLocations_ZoneId` (`ZoneId`),
  CONSTRAINT `FK_StorageLocations_Warehouses_WarehouseId` FOREIGN KEY (`WarehouseId`) REFERENCES `Warehouses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_StorageLocations_WarehouseZones_ZoneId` FOREIGN KEY (`ZoneId`) REFERENCES `WarehouseZones` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `StorageLocations`
--

LOCK TABLES `StorageLocations` WRITE;
/*!40000 ALTER TABLE `StorageLocations` DISABLE KEYS */;
INSERT INTO `StorageLocations` VALUES ('a1b2c3d4-1010-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','a1b2c3d4-1002-1111-2222-333333333333','A-01-01-01',NULL,'A','01','01','01',0,500.000000000000000000000000000000,2.000000000000000000000000000000,100.000000000000000000000000000000,0.500000000000000000000000000000,0,NULL,1,'2026-01-31 22:11:06.000000',NULL),('a1b2c3d4-1011-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','a1b2c3d4-1002-1111-2222-333333333333','A-01-01-02',NULL,'A','01','01','02',0,500.000000000000000000000000000000,2.000000000000000000000000000000,0.000000000000000000000000000000,0.000000000000000000000000000000,0,NULL,1,'2026-01-31 22:11:06.000000',NULL),('a1b2c3d4-1012-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','a1b2c3d4-1002-1111-2222-333333333333','A-01-02-01',NULL,'A','01','02','01',0,500.000000000000000000000000000000,2.000000000000000000000000000000,200.000000000000000000000000000000,1.000000000000000000000000000000,0,NULL,1,'2026-01-31 22:11:06.000000',NULL),('a1b2c3d4-1013-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','a1b2c3d4-1003-1111-2222-333333333333','B-01-01-01',NULL,'B','01','01','01',0,1000.000000000000000000000000000000,4.000000000000000000000000000000,300.000000000000000000000000000000,1.500000000000000000000000000000,0,NULL,1,'2026-01-31 22:11:06.000000',NULL),('a1b2c3d4-1014-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','a1b2c3d4-1003-1111-2222-333333333333','B-01-01-02',NULL,'B','01','01','02',0,1000.000000000000000000000000000000,4.000000000000000000000000000000,0.000000000000000000000000000000,0.000000000000000000000000000000,0,NULL,1,'2026-01-31 22:11:06.000000',NULL),('e0000001-1111-1111-1111-111111111111','11111111-1111-1111-1111-111111111111','d0000001-2222-2222-2222-222222222222','A-01-01-01','Prateleira A nivel 1','A','R01','L01','P01',1,500.000000000000000000000000000000,2.000000000000000000000000000000,150.000000000000000000000000000000,0.500000000000000000000000000000,0,NULL,1,'2026-01-31 22:18:54.000000',NULL),('e0000001-2222-2222-2222-222222222222','11111111-1111-1111-1111-111111111111','d0000001-2222-2222-2222-222222222222','A-01-01-02','Prateleira A nivel 2','A','R01','L01','P02',1,500.000000000000000000000000000000,2.000000000000000000000000000000,200.000000000000000000000000000000,0.700000000000000000000000000000,0,NULL,1,'2026-01-31 22:18:54.000000',NULL),('e0000001-3333-3333-3333-333333333333','11111111-1111-1111-1111-111111111111','d0000001-2222-2222-2222-222222222222','A-01-02-01','Prateleira A nivel 3','A','R01','L02','P01',1,300.000000000000000000000000000000,1.500000000000000000000000000000,100.000000000000000000000000000000,0.300000000000000000000000000000,0,NULL,1,'2026-01-31 22:18:54.000000',NULL),('e0000001-4444-4444-4444-444444444444','11111111-1111-1111-1111-111111111111','d0000001-3333-3333-3333-333333333333','B-01-01-01','Prateleira B nivel 1','B','R01','L01','P01',1,600.000000000000000000000000000000,2.500000000000000000000000000000,300.000000000000000000000000000000,1.000000000000000000000000000000,0,NULL,1,'2026-01-31 22:18:54.000000',NULL),('e0000001-5555-5555-5555-555555555555','11111111-1111-1111-1111-111111111111','d0000001-5555-5555-5555-555555555555','C-01-01-01','Area refrigerada 1','C','R01','L01','P01',2,200.000000000000000000000000000000,1.000000000000000000000000000000,50.000000000000000000000000000000,0.200000000000000000000000000000,0,NULL,1,'2026-01-31 22:18:54.000000',NULL);
/*!40000 ALTER TABLE `StorageLocations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Suppliers`
--

DROP TABLE IF EXISTS `Suppliers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Suppliers` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Document` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Address` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Suppliers_Document` (`Document`),
  KEY `IX_Suppliers_CompanyId` (`CompanyId`),
  CONSTRAINT `FK_Suppliers_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Suppliers`
--

LOCK TABLES `Suppliers` WRITE;
/*!40000 ALTER TABLE `Suppliers` DISABLE KEYS */;
INSERT INTO `Suppliers` VALUES ('22222222-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','Fornecedor ABC Ltda','12345678000101','(11) 99999-0001','joao@fornecedorabc.com',NULL,1,'2026-01-31 22:02:21.000000',NULL),('22222222-2222-2222-2222-222222222222','a1b2c3d4-5555-1111-2222-333333333333','Distribuidora XYZ','12345678000102','(11) 98888-0002','maria@xyz.com.br',NULL,1,'2026-01-31 22:02:21.000000',NULL),('22222222-3333-3333-3333-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Importadora Global','12345678000103','(11) 97777-0003','pedro@globalimport.com',NULL,1,'2026-01-31 22:02:21.000000',NULL),('a1b2c3d4-0001-0001-0001-000000000001','a1b2c3d4-5555-1111-2222-333333333333','TechParts Global','11111111000111','11999990001','contato@techparts.com','Av. Tech, 100',1,'2026-01-31 22:04:59.000000',NULL),('a1b2c3d4-0001-0001-0001-000000000002','a1b2c3d4-5555-1111-2222-333333333333','LogiSupply Europe','22222222000122','11999990002','sales@logisupply.eu','Rua Europa, 200',1,'2026-01-31 22:04:59.000000',NULL),('a1b2c3d4-0001-0001-0001-000000000003','a1b2c3d4-5555-1111-2222-333333333333','FastComponents Ltd','33333333000133','11999990003','info@fastcomp.com','Av. Industrial, 300',1,'2026-01-31 22:04:59.000000',NULL);
/*!40000 ALTER TABLE `Suppliers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Users` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PasswordHash` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Role` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `LastLoginAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Users_Email` (`Email`),
  KEY `IX_Users_CompanyId` (`CompanyId`),
  CONSTRAINT `FK_Users_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Users`
--

LOCK TABLES `Users` WRITE;
/*!40000 ALTER TABLE `Users` DISABLE KEYS */;
INSERT INTO `Users` VALUES ('ab3c484a-55c9-479c-8b31-ddbc7aed26d4','a1b2c3d4-5555-1111-2222-333333333333','Admin','admin@nexus.com','$2a$11$/0hvit4QmSxyP0hwch79XuV6NN6IDP3Bolc9AE3q2yzEWlEseS5RG',0,1,'2026-01-31 21:50:54.861046',NULL,'2026-02-01 09:54:18.529648'),('f6eb50c3-1df3-438f-96cd-02a93d771e6c',NULL,'Admin','admin@WMS.com','$2a$11$CC7wIVDETOLetiNvANi5QOLnhvaZgCI5E9ULXgNxqB6GwD0cbn2rS',0,1,'2026-02-01 09:32:19.162672',NULL,'2026-02-01 10:16:09.606348');
/*!40000 ALTER TABLE `Users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `VehicleAppointments`
--

DROP TABLE IF EXISTS `VehicleAppointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `VehicleAppointments` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `AppointmentNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `WarehouseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `VehicleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DriverId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DockDoorId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `Type` int NOT NULL,
  `ScheduledDate` datetime(6) NOT NULL,
  `ArrivalDate` datetime(6) DEFAULT NULL,
  `DepartureDate` datetime(6) DEFAULT NULL,
  `Status` int NOT NULL,
  `ServiceTime` time(6) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_VehicleAppointments_DockDoorId` (`DockDoorId`),
  KEY `IX_VehicleAppointments_DriverId` (`DriverId`),
  KEY `IX_VehicleAppointments_VehicleId` (`VehicleId`),
  KEY `IX_VehicleAppointments_WarehouseId` (`WarehouseId`),
  CONSTRAINT `FK_VehicleAppointments_DockDoors_DockDoorId` FOREIGN KEY (`DockDoorId`) REFERENCES `DockDoors` (`Id`),
  CONSTRAINT `FK_VehicleAppointments_Drivers_DriverId` FOREIGN KEY (`DriverId`) REFERENCES `Drivers` (`Id`),
  CONSTRAINT `FK_VehicleAppointments_Vehicles_VehicleId` FOREIGN KEY (`VehicleId`) REFERENCES `Vehicles` (`Id`),
  CONSTRAINT `FK_VehicleAppointments_Warehouses_WarehouseId` FOREIGN KEY (`WarehouseId`) REFERENCES `Warehouses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `VehicleAppointments`
--

LOCK TABLES `VehicleAppointments` WRITE;
/*!40000 ALTER TABLE `VehicleAppointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `VehicleAppointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `VehicleDamages`
--

DROP TABLE IF EXISTS `VehicleDamages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `VehicleDamages` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `VehicleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Type` int NOT NULL,
  `Severity` int NOT NULL,
  `DamageLocation` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `OccurrenceDate` datetime(6) NOT NULL,
  `ReportedDate` datetime(6) DEFAULT NULL,
  `RepairedDate` datetime(6) DEFAULT NULL,
  `MileageAtOccurrence` decimal(12,2) NOT NULL,
  `Status` int NOT NULL,
  `EstimatedRepairCost` decimal(12,2) NOT NULL,
  `ActualRepairCost` decimal(12,2) NOT NULL,
  `DriverId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DriverName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsThirdPartyFault` tinyint(1) NOT NULL,
  `ThirdPartyInfo` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `InsuranceClaim` tinyint(1) NOT NULL,
  `InsuranceClaimNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `InsuranceReimbursement` decimal(12,2) DEFAULT NULL,
  `RepairShop` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `RepairNotes` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PhotoUrls` varchar(4000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Notes` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_VehicleDamages_CompanyId` (`CompanyId`),
  KEY `IX_VehicleDamages_DriverId` (`DriverId`),
  KEY `IX_VehicleDamages_OccurrenceDate` (`OccurrenceDate`),
  KEY `IX_VehicleDamages_Status` (`Status`),
  KEY `IX_VehicleDamages_VehicleId` (`VehicleId`),
  KEY `IX_VehicleDamages_VehicleId_OccurrenceDate` (`VehicleId`,`OccurrenceDate`),
  CONSTRAINT `FK_VehicleDamages_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleDamages_Drivers_DriverId` FOREIGN KEY (`DriverId`) REFERENCES `Drivers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleDamages_Vehicles_VehicleId` FOREIGN KEY (`VehicleId`) REFERENCES `Vehicles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `VehicleDamages`
--

LOCK TABLES `VehicleDamages` WRITE;
/*!40000 ALTER TABLE `VehicleDamages` DISABLE KEYS */;
INSERT INTO `VehicleDamages` VALUES ('9fffb86d-ff57-11f0-ad5f-52121ab84f7c','55555555-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','Amassado lateral','Amassado causado por colisÃ£o leve',2,1,'Lateral direita','2026-01-15 10:00:00.000000','2026-01-15 10:30:00.000000',NULL,45000.00,0,800.00,0.00,NULL,'JoÃ£o Silva',0,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,'2026-02-01 10:20:26.000000',NULL),('9fffe7e3-ff57-11f0-ad5f-52121ab84f7c','55555555-2222-2222-2222-222222222222','a1b2c3d4-5555-1111-2222-333333333333','Para-brisa trincado','Pedra na estrada causou trinca',3,2,'Para-brisa dianteiro','2026-01-20 14:00:00.000000','2026-01-20 14:15:00.000000',NULL,62000.00,2,1200.00,0.00,NULL,'Carlos Pereira',0,NULL,1,NULL,NULL,NULL,NULL,NULL,NULL,'2026-02-01 10:20:26.000000',NULL),('9fffead2-ff57-11f0-ad5f-52121ab84f7c','55555555-3333-3333-3333-333333333333','a1b2c3d4-5555-1111-2222-333333333333','ArranhÃµes pintura','ArranhÃµes na pintura lateral esquerda',1,0,'Lateral esquerda','2026-01-10 09:00:00.000000','2026-01-10 09:30:00.000000',NULL,73000.00,3,450.00,380.00,NULL,'Pedro Santos',0,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,'2026-02-01 10:20:26.000000',NULL),('9fffec9f-ff57-11f0-ad5f-52121ab84f7c','55555555-4444-4444-4444-444444444444','a1b2c3d4-5555-1111-2222-333333333333','ColisÃ£o traseira','Terceiro bateu no semÃ¡foro',0,2,'Para-choque traseiro','2026-01-05 08:00:00.000000','2026-01-05 08:30:00.000000',NULL,78500.00,3,5500.00,4800.00,NULL,'Marcos Lima',1,NULL,1,NULL,NULL,NULL,NULL,NULL,NULL,'2026-02-01 10:20:26.000000',NULL),('9ffff715-ff57-11f0-ad5f-52121ab84f7c','a1b2c3d4-5001-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Farol quebrado','Farol dianteiro esquerdo quebrado',3,1,'Farol dianteiro esquerdo','2026-01-25 16:00:00.000000','2026-01-25 16:15:00.000000',NULL,30000.00,0,350.00,0.00,NULL,'Ana Costa',0,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,'2026-02-01 10:20:26.000000',NULL),('9ffff90b-ff57-11f0-ad5f-52121ab84f7c','a1b2c3d4-5002-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Retrovisor danificado','Retrovisor direito arrancado',2,1,'Retrovisor direito','2026-01-22 11:00:00.000000','2026-01-22 11:30:00.000000',NULL,44000.00,2,280.00,0.00,NULL,'Bruno Oliveira',0,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,'2026-02-01 10:20:26.000000',NULL),('9ffffb26-ff57-11f0-ad5f-52121ab84f7c','a1b2c3d4-5003-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Pneu furado','Pneu traseiro furado por prego',4,0,'Pneu traseiro direito','2026-01-28 07:30:00.000000','2026-01-28 08:00:00.000000',NULL,54000.00,3,150.00,120.00,NULL,'Ricardo Souza',0,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,'2026-02-01 10:20:26.000000',NULL),('9ffffd32-ff57-11f0-ad5f-52121ab84f7c','a1b2c3d4-5004-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Porta amassada','Porta traseira esquerda amassada',2,1,'Porta traseira esquerda','2026-01-18 13:00:00.000000','2026-01-18 13:20:00.000000',NULL,50500.00,1,600.00,0.00,NULL,'Fernando Alves',0,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,'2026-02-01 10:20:26.000000',NULL);
/*!40000 ALTER TABLE `VehicleDamages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `VehicleDocuments`
--

DROP TABLE IF EXISTS `VehicleDocuments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `VehicleDocuments` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `VehicleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Type` int NOT NULL,
  `DocumentNumber` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IssueDate` datetime(6) NOT NULL,
  `ExpiryDate` datetime(6) DEFAULT NULL,
  `IssuingAuthority` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `FileName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `FilePath` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `FileType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Cost` decimal(18,2) DEFAULT NULL,
  `AlertOnExpiry` tinyint(1) NOT NULL DEFAULT '1',
  `AlertDaysBefore` int DEFAULT '30',
  `Notes` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_VehicleDocuments_CompanyId` (`CompanyId`),
  KEY `IX_VehicleDocuments_ExpiryDate` (`ExpiryDate`),
  KEY `IX_VehicleDocuments_VehicleId` (`VehicleId`),
  CONSTRAINT `FK_VehicleDocuments_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleDocuments_Vehicles_VehicleId` FOREIGN KEY (`VehicleId`) REFERENCES `Vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `VehicleDocuments`
--

LOCK TABLES `VehicleDocuments` WRITE;
/*!40000 ALTER TABLE `VehicleDocuments` DISABLE KEYS */;
INSERT INTO `VehicleDocuments` VALUES ('a1b2c3d4-6002-1111-2222-333333333333','a1b2c3d4-5001-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',0,'CRLV-2025-002345','CRLV - Certificado de Registro','2025-02-10 00:00:00.000000','2026-02-10 00:00:00.000000','DETRAN-SP',NULL,NULL,NULL,280.00,1,30,NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6003-1111-2222-333333333333','a1b2c3d4-5001-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',10,'SEG-2025-6789','Seguro Total - Bradesco','2025-04-15 00:00:00.000000','2026-04-15 00:00:00.000000','Bradesco Seguros',NULL,NULL,NULL,5200.00,1,45,NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6004-1111-2222-333333333333','a1b2c3d4-5002-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',0,'CRLV-2025-003456','CRLV - Certificado de Registro','2025-03-05 00:00:00.000000','2026-03-05 00:00:00.000000','DETRAN-SP',NULL,NULL,NULL,320.00,1,30,NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6005-1111-2222-333333333333','a1b2c3d4-5002-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',10,'SEG-2025-7890','Seguro Total - SulAmérica','2025-05-01 00:00:00.000000','2026-05-01 00:00:00.000000','SulAmérica',NULL,NULL,NULL,7200.00,1,45,NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6006-1111-2222-333333333333','a1b2c3d4-5002-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',20,'ANTT-2025-001','Licença ANTT','2025-01-01 00:00:00.000000','2026-01-01 00:00:00.000000','ANTT',NULL,NULL,NULL,850.00,1,60,NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6007-1111-2222-333333333333','a1b2c3d4-5003-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',0,'CRLV-2025-004567','CRLV - Certificado de Registro','2025-04-01 00:00:00.000000','2026-04-01 00:00:00.000000','DETRAN-SP',NULL,NULL,NULL,350.00,1,30,NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6008-1111-2222-333333333333','a1b2c3d4-5003-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',10,'SEG-2025-8901','Seguro Total - Mapfre','2025-06-01 00:00:00.000000','2026-06-01 00:00:00.000000','Mapfre Seguros',NULL,NULL,NULL,8500.00,1,45,NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6009-1111-2222-333333333333','a1b2c3d4-5004-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',0,'CRLV-2025-005678','CRLV - Certificado de Registro','2025-05-10 00:00:00.000000','2026-05-10 00:00:00.000000','DETRAN-SP',NULL,NULL,NULL,220.00,1,30,NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6010-1111-2222-333333333333','a1b2c3d4-5004-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',10,'SEG-2025-9012','Seguro Total - Tokio Marine','2025-07-01 00:00:00.000000','2026-07-01 00:00:00.000000','Tokio Marine',NULL,NULL,NULL,2800.00,1,45,NULL,'2026-01-31 22:12:50.000000',NULL);
/*!40000 ALTER TABLE `VehicleDocuments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `VehicleInspections`
--

DROP TABLE IF EXISTS `VehicleInspections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `VehicleInspections` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `VehicleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Type` int NOT NULL,
  `InspectionDate` datetime(6) NOT NULL,
  `ExpiryDate` datetime(6) NOT NULL,
  `Result` int NOT NULL,
  `InspectionCenter` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `InspectorName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CertificateNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `MileageAtInspection` decimal(12,2) NOT NULL,
  `Cost` decimal(18,2) NOT NULL,
  `Observations` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DefectsFound` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_VehicleInspections_CompanyId` (`CompanyId`),
  KEY `IX_VehicleInspections_ExpiryDate` (`ExpiryDate`),
  KEY `IX_VehicleInspections_VehicleId` (`VehicleId`),
  CONSTRAINT `FK_VehicleInspections_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleInspections_Vehicles_VehicleId` FOREIGN KEY (`VehicleId`) REFERENCES `Vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `VehicleInspections`
--

LOCK TABLES `VehicleInspections` WRITE;
/*!40000 ALTER TABLE `VehicleInspections` DISABLE KEYS */;
INSERT INTO `VehicleInspections` VALUES ('a1b2c3d4-6201-1111-2222-333333333333','a1b2c3d4-5001-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',6,'2025-07-20 00:00:00.000000','2026-07-20 00:00:00.000000',0,'DETRAN-SP Inspeção','Roberto Santos','INSP-2025-002345',40000.00,220.00,'Aprovado',NULL,'2026-01-31 22:13:18.000000',NULL),('a1b2c3d4-6202-1111-2222-333333333333','a1b2c3d4-5002-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',6,'2025-08-10 00:00:00.000000','2026-08-10 00:00:00.000000',0,'DETRAN-SP Inspeção','Marcos Pereira','INSP-2025-003456',70000.00,250.00,'Aprovado',NULL,'2026-01-31 22:13:18.000000',NULL),('a1b2c3d4-6203-1111-2222-333333333333','a1b2c3d4-5003-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',6,'2025-09-05 00:00:00.000000','2026-09-05 00:00:00.000000',0,'DETRAN-SP Inspeção','Pedro Gonçalves','INSP-2025-004567',48000.00,250.00,'Aprovado',NULL,'2026-01-31 22:13:18.000000',NULL),('a1b2c3d4-6204-1111-2222-333333333333','a1b2c3d4-5004-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',6,'2025-10-01 00:00:00.000000','2026-10-01 00:00:00.000000',0,'DETRAN-SP Inspeção','Fernanda Alves','INSP-2025-005678',6000.00,160.00,'Aprovado',NULL,'2026-01-31 22:13:18.000000',NULL);
/*!40000 ALTER TABLE `VehicleInspections` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `VehicleMaintenances`
--

DROP TABLE IF EXISTS `VehicleMaintenances`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `VehicleMaintenances` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `VehicleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Type` int NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MaintenanceDate` datetime(6) NOT NULL,
  `NextMaintenanceDate` datetime(6) DEFAULT NULL,
  `MileageAtMaintenance` decimal(12,2) NOT NULL,
  `NextMaintenanceMileage` decimal(12,2) DEFAULT NULL,
  `LaborCost` decimal(18,2) NOT NULL,
  `PartsCost` decimal(18,2) NOT NULL,
  `ServiceProvider` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ServiceProviderContact` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `InvoiceNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Notes` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_VehicleMaintenances_CompanyId` (`CompanyId`),
  KEY `IX_VehicleMaintenances_MaintenanceDate` (`MaintenanceDate`),
  KEY `IX_VehicleMaintenances_VehicleId` (`VehicleId`),
  CONSTRAINT `FK_VehicleMaintenances_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleMaintenances_Vehicles_VehicleId` FOREIGN KEY (`VehicleId`) REFERENCES `Vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `VehicleMaintenances`
--

LOCK TABLES `VehicleMaintenances` WRITE;
/*!40000 ALTER TABLE `VehicleMaintenances` DISABLE KEYS */;
INSERT INTO `VehicleMaintenances` VALUES ('a1b2c3d4-6101-1111-2222-333333333333','a1b2c3d4-5001-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',2,'Troca de óleo motor','2025-10-01 00:00:00.000000','2026-01-01 00:00:00.000000',44000.00,54000.00,150.00,320.00,'Rede Iveco',NULL,'NF-2025-2345',NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6102-1111-2222-333333333333','a1b2c3d4-5001-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',4,'Revisão sistema de freios','2025-09-15 00:00:00.000000','2026-03-15 00:00:00.000000',42000.00,62000.00,350.00,850.00,'Freios e Cia',NULL,'NF-2025-2346',NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6103-1111-2222-333333333333','a1b2c3d4-5002-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',2,'Troca de óleo e filtros','2025-12-01 00:00:00.000000','2026-03-01 00:00:00.000000',77000.00,87000.00,200.00,480.00,'Mercedes-Benz Service',NULL,'NF-2025-3456',NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6104-1111-2222-333333333333','a1b2c3d4-5003-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',0,'Revisão preventiva 50.000km','2025-11-01 00:00:00.000000','2026-05-01 00:00:00.000000',50000.00,60000.00,450.00,620.00,'Rede VW Caminhões',NULL,'NF-2025-4567',NULL,'2026-01-31 22:12:50.000000',NULL),('a1b2c3d4-6105-1111-2222-333333333333','a1b2c3d4-5004-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',2,'Troca de óleo','2025-12-15 00:00:00.000000','2026-03-15 00:00:00.000000',7500.00,17500.00,80.00,180.00,'Fiat Autorizada',NULL,'NF-2025-5678',NULL,'2026-01-31 22:12:50.000000',NULL);
/*!40000 ALTER TABLE `VehicleMaintenances` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `VehicleMileageLogs`
--

DROP TABLE IF EXISTS `VehicleMileageLogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `VehicleMileageLogs` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `VehicleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Type` int NOT NULL,
  `StartMileage` decimal(12,2) NOT NULL,
  `EndMileage` decimal(12,2) NOT NULL,
  `StartLatitude` double DEFAULT NULL,
  `StartLongitude` double DEFAULT NULL,
  `StartAddress` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EndLatitude` double DEFAULT NULL,
  `EndLongitude` double DEFAULT NULL,
  `EndAddress` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `StartDateTime` datetime(6) NOT NULL,
  `EndDateTime` datetime(6) DEFAULT NULL,
  `DriverId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DriverName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ShipmentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `ShipmentNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `FuelConsumed` decimal(10,2) DEFAULT NULL,
  `FuelCost` decimal(12,2) DEFAULT NULL,
  `Status` int NOT NULL,
  `TollCost` decimal(12,2) DEFAULT NULL,
  `ParkingCost` decimal(12,2) DEFAULT NULL,
  `OtherCosts` decimal(12,2) DEFAULT NULL,
  `Purpose` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Notes` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `RoutePolyline` varchar(10000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_VehicleMileageLogs_CompanyId` (`CompanyId`),
  KEY `IX_VehicleMileageLogs_DriverId` (`DriverId`),
  KEY `IX_VehicleMileageLogs_ShipmentId` (`ShipmentId`),
  KEY `IX_VehicleMileageLogs_StartDateTime` (`StartDateTime`),
  KEY `IX_VehicleMileageLogs_Status` (`Status`),
  KEY `IX_VehicleMileageLogs_VehicleId` (`VehicleId`),
  KEY `IX_VehicleMileageLogs_VehicleId_StartDateTime` (`VehicleId`,`StartDateTime`),
  CONSTRAINT `FK_VehicleMileageLogs_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleMileageLogs_Drivers_DriverId` FOREIGN KEY (`DriverId`) REFERENCES `Drivers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleMileageLogs_OutboundShipments_ShipmentId` FOREIGN KEY (`ShipmentId`) REFERENCES `OutboundShipments` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleMileageLogs_Vehicles_VehicleId` FOREIGN KEY (`VehicleId`) REFERENCES `Vehicles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `VehicleMileageLogs`
--

LOCK TABLES `VehicleMileageLogs` WRITE;
/*!40000 ALTER TABLE `VehicleMileageLogs` DISABLE KEYS */;
INSERT INTO `VehicleMileageLogs` VALUES ('a1b2c3d4-6301-1111-2222-333333333333','a1b2c3d4-5001-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',0,44500.00,44780.00,NULL,NULL,'CD São Paulo',NULL,NULL,'Cliente Ribeirão Preto','2026-01-19 05:00:00.000000','2026-01-19 10:30:00.000000',NULL,'João Pedro Oliveira',NULL,NULL,55.00,330.00,2,120.00,NULL,NULL,'Rota interior SP',NULL,NULL,'2026-01-31 22:13:18.000000',NULL),('a1b2c3d4-6302-1111-2222-333333333333','a1b2c3d4-5002-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',0,77500.00,77950.00,NULL,NULL,'CD São Paulo',NULL,NULL,'Cliente Porto Alegre','2026-01-18 04:00:00.000000','2026-01-18 18:00:00.000000',NULL,'Maria Aparecida Costa',NULL,NULL,90.00,540.00,2,180.00,NULL,NULL,'Rota Sul',NULL,NULL,'2026-01-31 22:13:18.000000',NULL),('a1b2c3d4-6303-1111-2222-333333333333','a1b2c3d4-5003-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',0,51500.00,51850.00,NULL,NULL,'CD São Paulo',NULL,NULL,'Cliente Rio de Janeiro','2026-01-21 05:00:00.000000','2026-01-21 11:00:00.000000',NULL,'Roberto Almeida Junior',NULL,NULL,70.00,420.00,2,95.00,NULL,NULL,'Rota RJ',NULL,NULL,'2026-01-31 22:13:18.000000',NULL),('a1b2c3d4-6304-1111-2222-333333333333','a1b2c3d4-5004-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333',0,7800.00,7880.00,NULL,NULL,'CD São Paulo',NULL,NULL,'Cliente Guarulhos','2026-01-22 09:00:00.000000','2026-01-22 11:00:00.000000',NULL,'Ana Paula Ferreira',NULL,NULL,10.00,60.00,2,0.00,NULL,NULL,'Entrega local',NULL,NULL,'2026-01-31 22:13:18.000000',NULL);
/*!40000 ALTER TABLE `VehicleMileageLogs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Vehicles`
--

DROP TABLE IF EXISTS `Vehicles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Vehicles` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `LicensePlate` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Model` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Year` int NOT NULL,
  `Status` int NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `Brand` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `VehicleType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Capacity` decimal(10,2) DEFAULT NULL,
  `Color` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `FuelType` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Notes` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `TrackingToken` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `TrackingEnabled` tinyint(1) NOT NULL DEFAULT '0',
  `LastLatitude` double DEFAULT NULL,
  `LastLongitude` double DEFAULT NULL,
  `LastLocationUpdate` datetime(6) DEFAULT NULL,
  `CurrentSpeed` double DEFAULT NULL,
  `CurrentAddress` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DriverName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DriverPhone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DriverId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `CurrentShipmentId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `IsMoving` tinyint(1) NOT NULL DEFAULT '0',
  `ChassisNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EngineNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PurchasePrice` decimal(18,2) DEFAULT NULL,
  `PurchaseDate` datetime(6) DEFAULT NULL,
  `CurrentValue` decimal(18,2) DEFAULT NULL,
  `CurrentMileage` decimal(12,2) NOT NULL DEFAULT '0.00',
  `TotalDistanceTraveled` decimal(12,2) NOT NULL DEFAULT '0.00',
  `InsuranceExpiryDate` datetime(6) DEFAULT NULL,
  `LicenseExpiryDate` datetime(6) DEFAULT NULL,
  `LastInspectionDate` datetime(6) DEFAULT NULL,
  `NextInspectionDate` datetime(6) DEFAULT NULL,
  `LastMaintenanceDate` datetime(6) DEFAULT NULL,
  `LastMaintenanceMileage` decimal(12,2) DEFAULT NULL,
  `TotalMaintenanceCost` decimal(18,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Vehicles_LicensePlate` (`LicensePlate`),
  KEY `IX_Vehicles_CompanyId` (`CompanyId`),
  KEY `IX_Vehicles_DriverId` (`DriverId`),
  KEY `IX_Vehicles_CurrentShipmentId` (`CurrentShipmentId`),
  CONSTRAINT `FK_Vehicles_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Vehicles_Drivers_DriverId` FOREIGN KEY (`DriverId`) REFERENCES `Drivers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Vehicles_OutboundShipments_CurrentShipmentId` FOREIGN KEY (`CurrentShipmentId`) REFERENCES `OutboundShipments` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Vehicles`
--

LOCK TABLES `Vehicles` WRITE;
/*!40000 ALTER TABLE `Vehicles` DISABLE KEYS */;
INSERT INTO `Vehicles` VALUES ('55555555-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','ABC1234','Sprinter 515',2022,1,'2026-01-31 22:02:37.000000','2026-01-31 22:48:37.128705','Mercedes-Benz','Van',2500.00,'Branco','Diesel',NULL,'TRK-0C1F86D1481B',0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,45000.00,0.00,NULL,NULL,NULL,NULL,NULL,NULL,0.00),('55555555-2222-2222-2222-222222222222','a1b2c3d4-5555-1111-2222-333333333333','DEF5678','Fiorino',2021,1,'2026-01-31 22:02:37.000000','2026-01-31 22:48:37.962341','Fiat','Van',800.00,'Branco','Flex',NULL,'TRK-C54F7EE0B2B3',0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,32000.00,0.00,NULL,NULL,NULL,NULL,NULL,NULL,0.00),('55555555-3333-3333-3333-333333333333','a1b2c3d4-5555-1111-2222-333333333333','GHI9012','HR 2.5',2023,1,'2026-01-31 22:02:37.000000',NULL,'Hyundai','Caminhonete',1500.00,'Prata','Diesel',NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,18000.00,0.00,NULL,NULL,NULL,NULL,NULL,NULL,0.00),('55555555-4444-4444-4444-444444444444','a1b2c3d4-5555-1111-2222-333333333333','JKL3456','Accelo 1016',2020,1,'2026-01-31 22:02:37.000000',NULL,'Mercedes-Benz','Caminhão',6000.00,'Branco','Diesel',NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,120000.00,0.00,NULL,NULL,NULL,NULL,NULL,NULL,0.00),('a1b2c3d4-5001-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','DEF4G56','Daily 35S14',2022,0,'2026-01-31 22:12:12.000000',NULL,'Iveco','Furgão',2000.00,'Prata','Diesel',NULL,NULL,1,NULL,NULL,NULL,NULL,NULL,'João Pedro Oliveira','11988776655','a1b2c3d4-4001-1111-2222-333333333333',NULL,0,NULL,NULL,NULL,NULL,NULL,45000.00,45000.00,NULL,NULL,NULL,NULL,NULL,NULL,5200.00),('a1b2c3d4-5002-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','GHI7J89','Accelo 815',2021,0,'2026-01-31 22:12:12.000000',NULL,'Mercedes-Benz','Caminhão',5000.00,'Azul','Diesel',NULL,NULL,1,NULL,NULL,NULL,NULL,NULL,'Maria Aparecida Costa','11977665544','a1b2c3d4-4002-1111-2222-333333333333',NULL,0,NULL,NULL,NULL,NULL,NULL,78000.00,78000.00,NULL,NULL,NULL,NULL,NULL,NULL,12000.00),('a1b2c3d4-5003-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','JKL0M12','Delivery 11.180',2022,0,'2026-01-31 22:12:12.000000',NULL,'Volkswagen','Caminhão',8000.00,'Branco','Diesel',NULL,NULL,1,NULL,NULL,NULL,NULL,NULL,'Roberto Almeida Junior','11966554433','a1b2c3d4-4003-1111-2222-333333333333',NULL,0,NULL,NULL,NULL,NULL,NULL,52000.00,52000.00,NULL,NULL,NULL,NULL,NULL,NULL,8500.00),('a1b2c3d4-5004-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','NOP3Q45','Toro Endurance',2024,0,'2026-01-31 22:12:12.000000',NULL,'Fiat','Pickup',800.00,'Vermelho','Flex',NULL,NULL,1,NULL,NULL,NULL,NULL,NULL,'Ana Paula Ferreira','11955443322','a1b2c3d4-4004-1111-2222-333333333333',NULL,0,NULL,NULL,NULL,NULL,NULL,8000.00,8000.00,NULL,NULL,NULL,NULL,NULL,NULL,500.00);
/*!40000 ALTER TABLE `Vehicles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WarehouseZones`
--

DROP TABLE IF EXISTS `WarehouseZones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WarehouseZones` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `WarehouseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ZoneName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Temperature` decimal(5,2) DEFAULT NULL,
  `Humidity` decimal(5,2) DEFAULT NULL,
  `TotalCapacity` decimal(18,2) NOT NULL,
  `UsedCapacity` decimal(18,2) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_WarehouseZones_WarehouseId_ZoneName` (`WarehouseId`,`ZoneName`),
  CONSTRAINT `FK_WarehouseZones_Warehouses_WarehouseId` FOREIGN KEY (`WarehouseId`) REFERENCES `Warehouses` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WarehouseZones`
--

LOCK TABLES `WarehouseZones` WRITE;
/*!40000 ALTER TABLE `WarehouseZones` DISABLE KEYS */;
INSERT INTO `WarehouseZones` VALUES ('a1b2c3d4-1001-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','Recebimento','Receiving',NULL,NULL,1000.00,200.00,1,'2026-01-31 22:11:06.000000',NULL),('a1b2c3d4-1002-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','Armazenagem A','Storage',NULL,NULL,2000.00,800.00,1,'2026-01-31 22:11:06.000000',NULL),('a1b2c3d4-1003-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','Armazenagem B','Storage',NULL,NULL,2000.00,500.00,1,'2026-01-31 22:11:06.000000',NULL),('a1b2c3d4-1004-1111-2222-333333333333','a1b2c3d4-1000-1111-2222-333333333333','Expedição','Shipping',NULL,NULL,1000.00,300.00,1,'2026-01-31 22:11:06.000000',NULL),('d0000001-1111-1111-1111-111111111111','11111111-1111-1111-1111-111111111111','Zona de Recebimento','Receiving',22.00,50.00,1000.00,200.00,1,'2026-01-31 22:18:37.000000',NULL),('d0000001-2222-2222-2222-222222222222','11111111-1111-1111-1111-111111111111','Zona de Estoque A','Storage',20.00,45.00,3000.00,800.00,1,'2026-01-31 22:18:37.000000',NULL),('d0000001-3333-3333-3333-333333333333','11111111-1111-1111-1111-111111111111','Zona de Estoque B','Storage',20.00,45.00,2000.00,500.00,1,'2026-01-31 22:18:37.000000',NULL),('d0000001-4444-4444-4444-444444444444','11111111-1111-1111-1111-111111111111','Zona de Expedicao','Shipping',22.00,50.00,2000.00,400.00,1,'2026-01-31 22:18:37.000000',NULL),('d0000001-5555-5555-5555-555555555555','11111111-1111-1111-1111-111111111111','Zona Refrigerada','ColdStorage',5.00,60.00,500.00,100.00,1,'2026-01-31 22:18:37.000000',NULL);
/*!40000 ALTER TABLE `WarehouseZones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Warehouses`
--

DROP TABLE IF EXISTS `Warehouses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Warehouses` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CompanyId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Address` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsActive` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `City` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Country` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Latitude` double DEFAULT NULL,
  `Longitude` double DEFAULT NULL,
  `State` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ZipCode` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Warehouses_Code` (`Code`),
  KEY `IX_Warehouses_CompanyId` (`CompanyId`),
  CONSTRAINT `FK_Warehouses_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Warehouses`
--

LOCK TABLES `Warehouses` WRITE;
/*!40000 ALTER TABLE `Warehouses` DISABLE KEYS */;
INSERT INTO `Warehouses` VALUES ('11111111-1111-1111-1111-111111111111','a1b2c3d4-5555-1111-2222-333333333333','Centro de Distribuição Principal','CD-001','Av. Industrial, 1000',1,'2026-01-31 22:02:21.000000',NULL,'São Paulo','Brasil',NULL,NULL,'SP','01310-100'),('a1b2c3d4-1000-1111-2222-333333333333','a1b2c3d4-5555-1111-2222-333333333333','Armazém Central SP','WH-001','Av. Industrial, 1000',1,'2026-01-31 22:10:37.000000',NULL,'São Paulo','Brasil',NULL,NULL,'SP','01000-000');
/*!40000 ALTER TABLE `Warehouses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__EFMigrationsHistory`
--

LOCK TABLES `__EFMigrationsHistory` WRITE;
/*!40000 ALTER TABLE `__EFMigrationsHistory` DISABLE KEYS */;
INSERT INTO `__EFMigrationsHistory` VALUES ('20251122174613_InitialCreateComplete','8.0.0'),('20251125212515_AddOrderStatusPriorityAndWMSFields','8.0.0'),('20251125222824_AddAddressFieldsToWarehouseAndCustomer','8.0.0'),('20251127181300_AddPurchaseOrderAndParcelTracking','8.0.0'),('20251127183849_AddSoftDeleteToOrderDocuments','8.0.0'),('20251127185514_SeparatePurchaseAndSalesOrders','8.0.0'),('20251127211256_AddMissingPurchaseOrderFields','8.0.0'),('20260130120000_AddVehicleTrackingFields','8.0.0'),('20260130140000_AddVehicleDriverShipmentRelationships','8.0.0'),('20260130232704_AddVehicleManagementTables','8.0.0'),('20260131000000_AddVehicleAdditionalFields','8.0.0');
/*!40000 ALTER TABLE `__EFMigrationsHistory` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-02-01 10:47:57
