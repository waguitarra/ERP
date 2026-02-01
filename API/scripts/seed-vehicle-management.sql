-- ============================================
-- SEED DATA - VEHICLE MANAGEMENT
-- Popula documentos, manutenções, inspeções, avarias e quilometragem para todos os veículos
-- ============================================

SET @company_id = '400812ed-61e1-4902-a7d7-1bcf30df3907';

-- ============================================
-- DOCUMENTOS (Cada veículo terá 3-4 documentos)
-- Type: 0=RegistrationCertificate, 1=OwnershipTitle, 10=Insurance, 11=ThirdPartyInsurance, 20=OperatingLicense, 30=RoadTax, 40=TachographCertificate
-- ============================================

-- Veículo 1: VIT1Q00 - Iveco Daily
INSERT INTO VehicleDocuments (Id, VehicleId, CompanyId, Type, DocumentNumber, Description, IssueDate, ExpiryDate, IssuingAuthority, Cost, AlertOnExpiry, AlertDaysBefore, Notes, CreatedAt) VALUES
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 0, 'CRLV-2025-001234', 'CRLV - Certificado de Registro e Licenciamento', '2025-01-15 10:00:00', '2026-01-15 23:59:59', 'DETRAN-SP', 250.00, 1, 30, 'Documento em dia', NOW()),
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 10, 'SEG-AUTO-2025-5678', 'Seguro Total - Porto Seguro', '2025-03-01 08:00:00', '2026-03-01 23:59:59', 'Porto Seguro', 4500.00, 1, 45, 'Cobertura completa + terceiros', NOW()),
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 20, 'ANTT-2024-78901', 'Licença de Operação ANTT', '2024-06-01 10:00:00', '2025-06-01 23:59:59', 'ANTT', 850.00, 1, 60, 'Autorização transporte de cargas', NOW()),
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 30, 'IPVA-2025-VIT1Q00', 'IPVA 2025 - Pago', '2025-01-05 09:00:00', '2025-12-31 23:59:59', 'SEFAZ-SP', 1200.00, 1, 30, 'Pagamento à vista com desconto', NOW());

-- Veículo 2: BRA2E19 - VW Delivery
INSERT INTO VehicleDocuments (Id, VehicleId, CompanyId, Type, DocumentNumber, Description, IssueDate, ExpiryDate, IssuingAuthority, Cost, AlertOnExpiry, AlertDaysBefore, Notes, CreatedAt) VALUES
(UUID(), 'a1b2c3d4-1111-2222-3333-111111111111', @company_id, 0, 'CRLV-2025-002345', 'CRLV - Certificado de Registro e Licenciamento', '2025-02-10 10:00:00', '2026-02-10 23:59:59', 'DETRAN-SP', 280.00, 1, 30, NULL, NOW()),
(UUID(), 'a1b2c3d4-1111-2222-3333-111111111111', @company_id, 10, 'SEG-AUTO-2025-6789', 'Seguro Total - Bradesco Seguros', '2025-04-15 08:00:00', '2026-04-15 23:59:59', 'Bradesco Seguros', 5200.00, 1, 45, 'Cobertura para carga inclusa', NOW()),
(UUID(), 'a1b2c3d4-1111-2222-3333-111111111111', @company_id, 40, 'TACO-2025-12345', 'Certificado Tacógrafo', '2025-01-20 14:00:00', '2026-01-20 23:59:59', 'INMETRO', 350.00, 1, 30, 'Calibração anual obrigatória', NOW());

-- Veículo 3: RIO3F28 - Mercedes Sprinter
INSERT INTO VehicleDocuments (Id, VehicleId, CompanyId, Type, DocumentNumber, Description, IssueDate, ExpiryDate, IssuingAuthority, Cost, AlertOnExpiry, AlertDaysBefore, Notes, CreatedAt) VALUES
(UUID(), 'a1b2c3d4-2222-2222-3333-222222222222', @company_id, 0, 'CRLV-2025-003456', 'CRLV - Certificado de Registro e Licenciamento', '2025-03-05 10:00:00', '2026-03-05 23:59:59', 'DETRAN-RJ', 290.00, 1, 30, NULL, NOW()),
(UUID(), 'a1b2c3d4-2222-2222-3333-222222222222', @company_id, 10, 'SEG-AUTO-2025-7890', 'Seguro Total - SulAmérica', '2025-05-01 08:00:00', '2026-05-01 23:59:59', 'SulAmérica', 6100.00, 1, 45, 'Cobertura premium', NOW()),
(UUID(), 'a1b2c3d4-2222-2222-3333-222222222222', @company_id, 30, 'IPVA-2025-RIO3F28', 'IPVA 2025', '2025-01-10 09:00:00', '2025-12-31 23:59:59', 'SEFAZ-RJ', 1850.00, 1, 30, NULL, NOW()),
(UUID(), 'a1b2c3d4-2222-2222-3333-222222222222', @company_id, 20, 'ANTT-2024-89012', 'Licença ANTT', '2024-08-15 10:00:00', '2025-08-15 23:59:59', 'ANTT', 850.00, 1, 60, NULL, NOW());

-- Veículo 4: MGA4H37 - Fiat Fiorino
INSERT INTO VehicleDocuments (Id, VehicleId, CompanyId, Type, DocumentNumber, Description, IssueDate, ExpiryDate, IssuingAuthority, Cost, AlertOnExpiry, AlertDaysBefore, Notes, CreatedAt) VALUES
(UUID(), 'a1b2c3d4-3333-2222-3333-333333333333', @company_id, 0, 'CRLV-2025-004567', 'CRLV', '2025-01-25 10:00:00', '2026-01-25 23:59:59', 'DETRAN-MG', 220.00, 1, 30, NULL, NOW()),
(UUID(), 'a1b2c3d4-3333-2222-3333-333333333333', @company_id, 10, 'SEG-AUTO-2025-8901', 'Seguro - Liberty', '2025-02-20 08:00:00', '2026-02-20 23:59:59', 'Liberty Seguros', 2800.00, 1, 45, NULL, NOW()),
(UUID(), 'a1b2c3d4-3333-2222-3333-333333333333', @company_id, 30, 'IPVA-2025-MGA4H37', 'IPVA 2025', '2025-01-08 09:00:00', '2025-12-31 23:59:59', 'SEFAZ-MG', 680.00, 1, 30, NULL, NOW());

-- Veículo 5: POA5J46 - Mercedes Accelo
INSERT INTO VehicleDocuments (Id, VehicleId, CompanyId, Type, DocumentNumber, Description, IssueDate, ExpiryDate, IssuingAuthority, Cost, AlertOnExpiry, AlertDaysBefore, Notes, CreatedAt) VALUES
(UUID(), 'a1b2c3d4-4444-2222-3333-444444444444', @company_id, 0, 'CRLV-2025-005678', 'CRLV', '2025-04-01 10:00:00', '2026-04-01 23:59:59', 'DETRAN-RS', 320.00, 1, 30, NULL, NOW()),
(UUID(), 'a1b2c3d4-4444-2222-3333-444444444444', @company_id, 10, 'SEG-AUTO-2025-9012', 'Seguro Total - Mapfre', '2025-06-01 08:00:00', '2026-06-01 23:59:59', 'Mapfre Seguros', 7200.00, 1, 45, 'Caminhão - cobertura especial', NOW()),
(UUID(), 'a1b2c3d4-4444-2222-3333-444444444444', @company_id, 20, 'ANTT-2024-90123', 'Licença ANTT', '2024-09-01 10:00:00', '2025-09-01 23:59:59', 'ANTT', 950.00, 1, 60, NULL, NOW()),
(UUID(), 'a1b2c3d4-4444-2222-3333-444444444444', @company_id, 40, 'TACO-2025-23456', 'Certificado Tacógrafo', '2025-02-15 14:00:00', '2026-02-15 23:59:59', 'INMETRO', 380.00, 1, 30, NULL, NOW());

-- Veículos 6-10 (dados simplificados)
INSERT INTO VehicleDocuments (Id, VehicleId, CompanyId, Type, DocumentNumber, Description, IssueDate, ExpiryDate, IssuingAuthority, Cost, AlertOnExpiry, AlertDaysBefore, Notes, CreatedAt) VALUES
-- CWB6K55 - Fiat Toro
(UUID(), 'a1b2c3d4-5555-2222-3333-555555555555', @company_id, 0, 'CRLV-2025-006789', 'CRLV', '2025-05-10 10:00:00', '2026-05-10 23:59:59', 'DETRAN-PR', 270.00, 1, 30, NULL, NOW()),
(UUID(), 'a1b2c3d4-5555-2222-3333-555555555555', @company_id, 10, 'SEG-AUTO-2025-0123', 'Seguro - Tokio Marine', '2025-07-01 08:00:00', '2026-07-01 23:59:59', 'Tokio Marine', 3500.00, 1, 45, NULL, NOW()),
-- REC7L64 - VW Saveiro
(UUID(), 'a1b2c3d4-6666-2222-3333-666666666666', @company_id, 0, 'CRLV-2025-007890', 'CRLV', '2025-06-15 10:00:00', '2026-06-15 23:59:59', 'DETRAN-PE', 240.00, 1, 30, NULL, NOW()),
(UUID(), 'a1b2c3d4-6666-2222-3333-666666666666', @company_id, 10, 'SEG-AUTO-2025-1234', 'Seguro - HDI', '2025-08-01 08:00:00', '2026-08-01 23:59:59', 'HDI Seguros', 2600.00, 1, 45, NULL, NOW()),
-- SAL8M73 - Hyundai HR
(UUID(), 'a1b2c3d4-7777-2222-3333-777777777777', @company_id, 0, 'CRLV-2025-008901', 'CRLV', '2025-07-20 10:00:00', '2026-07-20 23:59:59', 'DETRAN-BA', 260.00, 1, 30, NULL, NOW()),
(UUID(), 'a1b2c3d4-7777-2222-3333-777777777777', @company_id, 10, 'SEG-AUTO-2025-2345', 'Seguro - Allianz', '2025-09-01 08:00:00', '2026-09-01 23:59:59', 'Allianz Seguros', 3900.00, 1, 45, NULL, NOW()),
-- FOR9N82 - Kia Bongo
(UUID(), 'a1b2c3d4-8888-2222-3333-888888888888', @company_id, 0, 'CRLV-2025-009012', 'CRLV', '2025-08-25 10:00:00', '2026-08-25 23:59:59', 'DETRAN-CE', 250.00, 1, 30, NULL, NOW()),
(UUID(), 'a1b2c3d4-8888-2222-3333-888888888888', @company_id, 10, 'SEG-AUTO-2025-3456', 'Seguro - Zurich', '2025-10-01 08:00:00', '2026-10-01 23:59:59', 'Zurich Seguros', 3200.00, 1, 45, NULL, NOW()),
-- BSB0P91 - Renault Master
(UUID(), 'a1b2c3d4-9999-2222-3333-999999999999', @company_id, 0, 'CRLV-2025-010123', 'CRLV', '2025-09-30 10:00:00', '2026-09-30 23:59:59', 'DETRAN-DF', 300.00, 1, 30, NULL, NOW()),
(UUID(), 'a1b2c3d4-9999-2222-3333-999999999999', @company_id, 10, 'SEG-AUTO-2025-4567', 'Seguro - Itaú Seguros', '2025-11-01 08:00:00', '2026-11-01 23:59:59', 'Itaú Seguros', 5800.00, 1, 45, NULL, NOW()),
(UUID(), 'a1b2c3d4-9999-2222-3333-999999999999', @company_id, 20, 'ANTT-2024-01234', 'Licença ANTT', '2024-10-01 10:00:00', '2025-10-01 23:59:59', 'ANTT', 850.00, 1, 60, NULL, NOW());

-- ============================================
-- MANUTENÇÕES (Histórico de manutenções)
-- Type: 0=Preventive, 1=Corrective, 2=OilChange, 3=TireChange, 4=BrakeService, 5=EngineRepair, 7=Electrical, 9=AirConditioning
-- ============================================

INSERT INTO VehicleMaintenances (Id, VehicleId, CompanyId, Type, Description, MaintenanceDate, NextMaintenanceDate, MileageAtMaintenance, NextMaintenanceMileage, LaborCost, PartsCost, ServiceProvider, ServiceProviderContact, InvoiceNumber, Notes, CreatedAt) VALUES
-- Veículo 1: VIT1Q00
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 2, 'Troca de óleo e filtros - 10W40', '2025-09-15 08:00:00', '2025-12-15 08:00:00', 45000.00, 55000.00, 120.00, 280.00, 'Oficina Central Iveco', '(11) 3456-7890', 'NF-2025-1234', 'Óleo Mobil 1 10W40 sintético', NOW()),
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 4, 'Revisão de freios - pastilhas e discos', '2025-07-20 09:00:00', '2026-01-20 09:00:00', 38000.00, 58000.00, 350.00, 850.00, 'Freios e Cia', '(11) 2345-6789', 'NF-2025-0987', 'Pastilhas Bosch + Discos Hipper Freios', NOW()),
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 0, 'Revisão preventiva 40.000km', '2025-05-10 08:00:00', '2025-11-10 08:00:00', 40000.00, 50000.00, 450.00, 620.00, 'Oficina Central Iveco', '(11) 3456-7890', 'NF-2025-0654', 'Revisão completa conforme manual', NOW()),
-- Veículo 2: BRA2E19
(UUID(), 'a1b2c3d4-1111-2222-3333-111111111111', @company_id, 2, 'Troca de óleo motor e câmbio', '2025-10-01 08:00:00', '2026-01-01 08:00:00', 62000.00, 72000.00, 180.00, 420.00, 'Rede VW Autorizada', '(11) 4567-8901', 'NF-2025-2345', NULL, NOW()),
(UUID(), 'a1b2c3d4-1111-2222-3333-111111111111', @company_id, 3, 'Troca de pneus - 4 unidades', '2025-08-15 10:00:00', '2026-08-15 10:00:00', 55000.00, 95000.00, 200.00, 3200.00, 'Pneustore', '(11) 5678-9012', 'NF-2025-1876', 'Pneus Pirelli FR01 295/80R22.5', NOW()),
(UUID(), 'a1b2c3d4-1111-2222-3333-111111111111', @company_id, 1, 'Reparo no sistema elétrico', '2025-06-20 14:00:00', NULL, 48000.00, NULL, 280.00, 450.00, 'Eletro Auto', '(11) 6789-0123', 'NF-2025-0543', 'Substituição alternador', NOW()),
-- Veículo 3: RIO3F28
(UUID(), 'a1b2c3d4-2222-2222-3333-222222222222', @company_id, 2, 'Troca de óleo', '2025-11-01 08:00:00', '2026-02-01 08:00:00', 72000.00, 82000.00, 150.00, 350.00, 'Mercedes-Benz Service', '(21) 3456-7890', 'NF-2025-3456', NULL, NOW()),
(UUID(), 'a1b2c3d4-2222-2222-3333-222222222222', @company_id, 9, 'Manutenção ar condicionado', '2025-09-10 09:00:00', '2026-09-10 09:00:00', 68000.00, 88000.00, 200.00, 580.00, 'Ar Condicionado Automotivo', '(21) 4567-8901', 'NF-2025-2987', 'Recarga gás + limpeza evaporador', NOW()),
-- Veículo 4: MGA4H37
(UUID(), 'a1b2c3d4-3333-2222-3333-333333333333', @company_id, 2, 'Troca de óleo', '2025-10-20 08:00:00', '2026-01-20 08:00:00', 35000.00, 45000.00, 80.00, 180.00, 'Fiat Rede Autorizada', '(31) 2345-6789', 'NF-2025-4567', NULL, NOW()),
(UUID(), 'a1b2c3d4-3333-2222-3333-333333333333', @company_id, 0, 'Revisão 30.000km', '2025-08-05 08:00:00', '2026-02-05 08:00:00', 30000.00, 40000.00, 250.00, 380.00, 'Fiat Rede Autorizada', '(31) 2345-6789', 'NF-2025-3876', NULL, NOW()),
-- Veículo 5: POA5J46
(UUID(), 'a1b2c3d4-4444-2222-3333-444444444444', @company_id, 2, 'Troca de óleo e filtros', '2025-11-10 08:00:00', '2026-02-10 08:00:00', 85000.00, 95000.00, 200.00, 480.00, 'Mercedes-Benz POA', '(51) 3456-7890', 'NF-2025-5678', NULL, NOW()),
(UUID(), 'a1b2c3d4-4444-2222-3333-444444444444', @company_id, 4, 'Sistema de freios completo', '2025-09-25 09:00:00', '2026-03-25 09:00:00', 80000.00, 100000.00, 500.00, 1800.00, 'Freios Pesados RS', '(51) 4567-8901', 'NF-2025-4987', 'Lonas + tambores + reguladores', NOW()),
-- Veículos 6-10
(UUID(), 'a1b2c3d4-5555-2222-3333-555555555555', @company_id, 2, 'Troca de óleo', '2025-10-15 08:00:00', '2026-01-15 08:00:00', 28000.00, 38000.00, 100.00, 220.00, 'Oficina Fiat Curitiba', '(41) 2345-6789', 'NF-2025-6789', NULL, NOW()),
(UUID(), 'a1b2c3d4-6666-2222-3333-666666666666', @company_id, 2, 'Troca de óleo', '2025-09-20 08:00:00', '2025-12-20 08:00:00', 42000.00, 52000.00, 90.00, 190.00, 'VW Service Recife', '(81) 3456-7890', 'NF-2025-7890', NULL, NOW()),
(UUID(), 'a1b2c3d4-7777-2222-3333-777777777777', @company_id, 2, 'Troca de óleo', '2025-10-25 08:00:00', '2026-01-25 08:00:00', 52000.00, 62000.00, 110.00, 240.00, 'Hyundai Salvador', '(71) 4567-8901', 'NF-2025-8901', NULL, NOW()),
(UUID(), 'a1b2c3d4-8888-2222-3333-888888888888', @company_id, 2, 'Troca de óleo', '2025-11-05 08:00:00', '2026-02-05 08:00:00', 38000.00, 48000.00, 100.00, 210.00, 'Kia Motors Fortaleza', '(85) 5678-9012', 'NF-2025-9012', NULL, NOW()),
(UUID(), 'a1b2c3d4-9999-2222-3333-999999999999', @company_id, 2, 'Troca de óleo e filtros', '2025-10-30 08:00:00', '2026-01-30 08:00:00', 48000.00, 58000.00, 130.00, 290.00, 'Renault Brasília', '(61) 6789-0123', 'NF-2025-0123', NULL, NOW());

-- ============================================
-- INSPEÇÕES (ITV/DETRAN)
-- Type: 0=ITV, 6=DETRAN, 7=SafetyInspection, 8=EmissionsTest
-- Result: 0=Approved, 1=ApprovedWithDefects, 2=Rejected
-- ============================================

INSERT INTO VehicleInspections (Id, VehicleId, CompanyId, Type, InspectionDate, ExpiryDate, Result, InspectionCenter, InspectorName, CertificateNumber, MileageAtInspection, Cost, Observations, DefectsFound, CreatedAt) VALUES
-- Veículo 1: VIT1Q00
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 6, '2025-06-15 09:00:00', '2026-06-15 23:59:59', 0, 'DETRAN-SP Inspeção Veicular', 'Carlos Silva', 'INSP-2025-001234', 42000.00, 180.00, 'Veículo aprovado sem ressalvas', NULL, NOW()),
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 8, '2025-06-15 10:00:00', '2026-06-15 23:59:59', 0, 'CETESB SP', 'Ana Oliveira', 'EMIS-2025-001234', 42000.00, 95.00, 'Emissões dentro dos padrões', NULL, NOW()),
-- Veículo 2: BRA2E19
(UUID(), 'a1b2c3d4-1111-2222-3333-111111111111', @company_id, 6, '2025-07-20 09:00:00', '2026-07-20 23:59:59', 0, 'DETRAN-SP Inspeção Veicular', 'Roberto Santos', 'INSP-2025-002345', 58000.00, 220.00, 'Aprovado', NULL, NOW()),
-- Veículo 3: RIO3F28
(UUID(), 'a1b2c3d4-2222-2222-3333-222222222222', @company_id, 6, '2025-08-10 09:00:00', '2026-08-10 23:59:59', 1, 'DETRAN-RJ Vistoria', 'Marcos Pereira', 'INSP-2025-003456', 70000.00, 200.00, 'Aprovado com pequenos defeitos', 'Farol direito desregulado', NOW()),
-- Veículo 4: MGA4H37
(UUID(), 'a1b2c3d4-3333-2222-3333-333333333333', @company_id, 6, '2025-05-25 09:00:00', '2026-05-25 23:59:59', 0, 'DETRAN-MG Vistoria', 'José Lima', 'INSP-2025-004567', 32000.00, 150.00, 'Aprovado', NULL, NOW()),
-- Veículo 5: POA5J46
(UUID(), 'a1b2c3d4-4444-2222-3333-444444444444', @company_id, 6, '2025-09-05 09:00:00', '2026-09-05 23:59:59', 0, 'DETRAN-RS Vistoria', 'Pedro Gonçalves', 'INSP-2025-005678', 82000.00, 250.00, 'Caminhão aprovado', NULL, NOW()),
(UUID(), 'a1b2c3d4-4444-2222-3333-444444444444', @company_id, 7, '2025-09-05 10:00:00', '2026-09-05 23:59:59', 0, 'INMETRO Safety', 'Maria Costa', 'SAFE-2025-005678', 82000.00, 320.00, 'Inspeção de segurança aprovada', NULL, NOW()),
-- Veículos 6-10
(UUID(), 'a1b2c3d4-5555-2222-3333-555555555555', @company_id, 6, '2025-10-01 09:00:00', '2026-10-01 23:59:59', 0, 'DETRAN-PR Vistoria', 'Fernanda Alves', 'INSP-2025-006789', 30000.00, 160.00, 'Aprovado', NULL, NOW()),
(UUID(), 'a1b2c3d4-6666-2222-3333-666666666666', @company_id, 6, '2025-11-01 09:00:00', '2026-11-01 23:59:59', 0, 'DETRAN-PE Vistoria', 'Ricardo Souza', 'INSP-2025-007890', 44000.00, 150.00, 'Aprovado', NULL, NOW()),
(UUID(), 'a1b2c3d4-7777-2222-3333-777777777777', @company_id, 6, '2025-12-01 09:00:00', '2026-12-01 23:59:59', 0, 'DETRAN-BA Vistoria', 'Luciana Martins', 'INSP-2025-008901', 54000.00, 170.00, 'Aprovado', NULL, NOW()),
(UUID(), 'a1b2c3d4-8888-2222-3333-888888888888', @company_id, 6, '2026-01-10 09:00:00', '2027-01-10 23:59:59', 0, 'DETRAN-CE Vistoria', 'Paulo Ribeiro', 'INSP-2026-009012', 40000.00, 155.00, 'Aprovado', NULL, NOW()),
(UUID(), 'a1b2c3d4-9999-2222-3333-999999999999', @company_id, 6, '2026-01-15 09:00:00', '2027-01-15 23:59:59', 0, 'DETRAN-DF Vistoria', 'Cláudia Ferreira', 'INSP-2026-010123', 50000.00, 180.00, 'Aprovado', NULL, NOW());

-- ============================================
-- AVARIAS (Algumas avarias de exemplo)
-- Type: 0=Collision, 1=Scratch, 2=Dent, 3=BrokenGlass, 4=MechanicalFailure
-- Severity: 0=Minor, 1=Moderate, 2=Major, 3=Critical
-- Status: 0=Reported, 1=UnderAssessment, 2=UnderRepair, 3=Repaired, 4=WriteOff
-- ============================================

INSERT INTO VehicleDamages (Id, VehicleId, CompanyId, Title, Description, Type, Severity, DamageLocation, OccurrenceDate, ReportedDate, RepairedDate, MileageAtOccurrence, Status, EstimatedRepairCost, ActualRepairCost, DriverName, IsThirdPartyFault, InsuranceClaim, RepairShop, RepairNotes, Notes, CreatedAt) VALUES
-- Veículo 1: VIT1Q00 - Avaria reparada
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 'Amassado na lateral direita', 'Amassado causado por colisão leve em manobra de estacionamento', 2, 0, 'Lateral direita traseira', '2025-08-10 14:30:00', '2025-08-10 15:00:00', '2025-08-15 17:00:00', 43500.00, 3, 800.00, 650.00, 'João Motorista', 0, 0, 'Funilaria Express', 'Reparo com PDR', 'Ocorreu no CD São Paulo', NOW()),
-- Veículo 2: BRA2E19 - Avaria em reparo
(UUID(), 'a1b2c3d4-1111-2222-3333-111111111111', @company_id, 'Para-brisa trincado', 'Trinca no para-brisa causada por pedra na estrada', 3, 1, 'Para-brisa dianteiro', '2025-11-20 10:15:00', '2025-11-20 10:30:00', NULL, 61500.00, 2, 1200.00, 0.00, 'Carlos Entregador', 0, 1, 'Vidros Auto SP', NULL, 'Sinistro em andamento', NOW()),
-- Veículo 3: RIO3F28 - Avaria reportada
(UUID(), 'a1b2c3d4-2222-2222-3333-222222222222', @company_id, 'Arranhões na lateral', 'Múltiplos arranhões na pintura lateral esquerda', 1, 0, 'Lateral esquerda', '2025-12-05 16:45:00', '2025-12-05 17:00:00', NULL, 73000.00, 0, 450.00, 0.00, 'Pedro Motorista', 0, 0, NULL, NULL, 'Aguardando orçamento', NOW()),
-- Veículo 5: POA5J46 - Avaria grave reparada
(UUID(), 'a1b2c3d4-4444-2222-3333-444444444444', @company_id, 'Colisão traseira', 'Colisão traseira em semáforo - terceiro bateu', 0, 2, 'Traseira', '2025-07-15 08:20:00', '2025-07-15 08:45:00', '2025-08-20 16:00:00', 78000.00, 3, 8500.00, 7200.00, 'Marcos Caminhoneiro', 1, 1, 'Funilaria Pesados RS', 'Reparo completo da traseira', 'Seguro cobriu 100%', NOW());

-- ============================================
-- QUILOMETRAGEM (Registros de viagens)
-- Type: 0=Delivery, 1=Pickup, 2=Transfer, 3=Maintenance, 4=Other
-- Status: 0=Planned, 1=InProgress, 2=Completed, 3=Cancelled
-- ============================================

INSERT INTO VehicleMileageLogs (Id, VehicleId, CompanyId, Type, StartMileage, EndMileage, StartAddress, EndAddress, StartDateTime, EndDateTime, DriverName, FuelConsumed, FuelCost, Status, TollCost, ParkingCost, OtherCosts, Purpose, Notes, CreatedAt) VALUES
-- Veículo 1: VIT1Q00 - Viagens completadas
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 0, 44800.00, 44920.00, 'CD São Paulo - Guarulhos', 'Cliente ABC - Campinas', '2025-12-20 06:00:00', '2025-12-20 09:30:00', 'João Motorista', 25.00, 150.00, 2, 45.00, 0.00, 0.00, 'Entrega lote #12345', 'Entrega realizada com sucesso', NOW()),
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 0, 44920.00, 45050.00, 'CD São Paulo - Guarulhos', 'Cliente XYZ - Sorocaba', '2025-12-21 07:00:00', '2025-12-21 11:00:00', 'João Motorista', 28.00, 168.00, 2, 65.00, 10.00, 0.00, 'Entrega lote #12346', NULL, NOW()),
(UUID(), 'a1b2c3d4-0000-2222-3333-000000000000', @company_id, 0, 45050.00, 45200.00, 'CD São Paulo', 'Cliente DEF - Santos', '2025-12-22 06:30:00', '2025-12-22 10:00:00', 'Carlos Entregador', 32.00, 192.00, 2, 35.00, 15.00, 0.00, 'Entrega urgente', NULL, NOW()),
-- Veículo 2: BRA2E19
(UUID(), 'a1b2c3d4-1111-2222-3333-111111111111', @company_id, 0, 61500.00, 61780.00, 'CD São Paulo', 'Cliente GHI - Ribeirão Preto', '2025-12-19 05:00:00', '2025-12-19 10:30:00', 'Pedro Motorista', 55.00, 330.00, 2, 120.00, 0.00, 0.00, 'Rota interior SP', NULL, NOW()),
(UUID(), 'a1b2c3d4-1111-2222-3333-111111111111', @company_id, 1, 61780.00, 62050.00, 'Fornecedor JKL - Piracicaba', 'CD São Paulo', '2025-12-20 13:00:00', '2025-12-20 17:00:00', 'Pedro Motorista', 52.00, 312.00, 2, 85.00, 20.00, 0.00, 'Coleta de mercadorias', NULL, NOW()),
-- Veículo 3: RIO3F28
(UUID(), 'a1b2c3d4-2222-2222-3333-222222222222', @company_id, 0, 72500.00, 72650.00, 'CD Rio de Janeiro', 'Cliente MNO - Niterói', '2025-12-21 08:00:00', '2025-12-21 10:00:00', 'Marcos Entregador', 20.00, 120.00, 2, 25.00, 0.00, 0.00, 'Entrega expressa', NULL, NOW()),
-- Veículo 4: MGA4H37
(UUID(), 'a1b2c3d4-3333-2222-3333-333333333333', @company_id, 0, 35200.00, 35280.00, 'CD Belo Horizonte', 'Cliente PQR - Contagem', '2025-12-22 09:00:00', '2025-12-22 11:00:00', 'Ricardo Motorista', 12.00, 72.00, 2, 0.00, 0.00, 0.00, 'Entrega local', 'Trânsito leve', NOW()),
-- Veículo 5: POA5J46
(UUID(), 'a1b2c3d4-4444-2222-3333-444444444444', @company_id, 0, 85200.00, 85650.00, 'CD Porto Alegre', 'Cliente STU - Caxias do Sul', '2025-12-18 04:00:00', '2025-12-18 08:30:00', 'Fernando Caminhoneiro', 90.00, 540.00, 2, 75.00, 0.00, 0.00, 'Rota serra gaúcha', NULL, NOW()),
(UUID(), 'a1b2c3d4-4444-2222-3333-444444444444', @company_id, 0, 85650.00, 86100.00, 'Cliente STU - Caxias', 'CD Porto Alegre', '2025-12-18 14:00:00', '2025-12-18 18:30:00', 'Fernando Caminhoneiro', 88.00, 528.00, 2, 75.00, 25.00, 0.00, 'Retorno com coleta', NULL, NOW()),
-- Veículos 6-10 (uma viagem cada)
(UUID(), 'a1b2c3d4-5555-2222-3333-555555555555', @company_id, 0, 28500.00, 28580.00, 'CD Curitiba', 'Cliente VWX - São José dos Pinhais', '2025-12-20 10:00:00', '2025-12-20 11:30:00', 'Gustavo Motorista', 10.00, 60.00, 2, 0.00, 0.00, 0.00, 'Entrega local', NULL, NOW()),
(UUID(), 'a1b2c3d4-6666-2222-3333-666666666666', @company_id, 0, 42500.00, 42620.00, 'CD Recife', 'Cliente YZA - Olinda', '2025-12-21 09:00:00', '2025-12-21 11:00:00', 'André Entregador', 15.00, 90.00, 2, 0.00, 0.00, 0.00, 'Rota metropolitana', NULL, NOW()),
(UUID(), 'a1b2c3d4-7777-2222-3333-777777777777', @company_id, 0, 54200.00, 54350.00, 'CD Salvador', 'Cliente BCD - Lauro de Freitas', '2025-12-22 08:00:00', '2025-12-22 10:30:00', 'Thiago Motorista', 18.00, 108.00, 2, 15.00, 0.00, 0.00, 'Entrega matinal', NULL, NOW()),
(UUID(), 'a1b2c3d4-8888-2222-3333-888888888888', @company_id, 0, 40200.00, 40320.00, 'CD Fortaleza', 'Cliente EFG - Maracanaú', '2025-12-19 07:00:00', '2025-12-19 09:00:00', 'Lucas Entregador', 14.00, 84.00, 2, 0.00, 0.00, 0.00, 'Entrega industrial', NULL, NOW()),
(UUID(), 'a1b2c3d4-9999-2222-3333-999999999999', @company_id, 0, 50200.00, 50450.00, 'CD Brasília', 'Cliente HIJ - Taguatinga', '2025-12-20 06:00:00', '2025-12-20 08:30:00', 'Rafael Motorista', 35.00, 210.00, 2, 0.00, 10.00, 0.00, 'Entrega DF', NULL, NOW());

SELECT 'Seed data inserted successfully!' as Status;
SELECT COUNT(*) as TotalDocuments FROM VehicleDocuments;
SELECT COUNT(*) as TotalMaintenances FROM VehicleMaintenances;
SELECT COUNT(*) as TotalInspections FROM VehicleInspections;
SELECT COUNT(*) as TotalDamages FROM VehicleDamages;
SELECT COUNT(*) as TotalMileageLogs FROM VehicleMileageLogs;
