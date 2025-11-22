#!/usr/bin/env python3
import mysql.connector
import sys

try:
    conn = mysql.connector.connect(
        host='localhost',
        user='logistics_user',
        password='password',
        database='logistics_db'
    )
    cursor = conn.cursor()
    
    tables = [
        'Companies', 'Users', 'Warehouses', 'WarehouseZones', 'DockDoors',
        'Suppliers', 'Customers', 'Products', 'Vehicles', 'Drivers',
        'StorageLocations', 'Orders', 'OrderItems', 'VehicleAppointments',
        'InboundShipments', 'Receipts', 'ReceiptLines', 'Lots',
        'Inventories', 'StockMovements', 'PutawayTasks', 'PickingWaves',
        'PickingTasks', 'PickingLines', 'PackingTasks', 'Packages',
        'OutboundShipments', 'SerialNumbers', 'CycleCounts'
    ]
    
    print("=" * 60)
    print("CONTAGEM DE REGISTROS - 30 TABELAS")
    print("=" * 60)
    
    for table in tables:
        cursor.execute(f"SELECT COUNT(*) FROM {table}")
        count = cursor.fetchone()[0]
        status = "✅" if count > 0 else "❌"
        print(f"{status} {table:25} {count:5}")
    
    cursor.close()
    conn.close()
    
except Exception as e:
    print(f"ERRO: {e}")
    sys.exit(1)
