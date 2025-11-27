namespace Logistics.Domain.Enums;

public enum DocumentType
{
    Invoice = 1,                    // Fatura comercial
    PackingList = 2,                // Lista de embalagem
    ImportDeclaration = 3,          // DI - Declaração de Importação
    BillOfLading = 4,               // BL - Conhecimento de embarque
    ImportLicense = 5,              // LI - Licença de Importação
    CertificateOfOrigin = 6,        // Certificado de Origem
    CustomsDocumentation = 7,       // Documentos aduaneiros
    Other = 99                      // Outros
}
