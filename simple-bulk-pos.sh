#!/bin/bash

# Simple bulk create purchase orders
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiNTJlNWZiZC1jMzFhLTQ5YTAtYjc4Zi0zYmExNzQ2ZTVmM2QiLCJlbWFpbCI6ImFkbWluQG5leHVzLmNvbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiOGI5YjBiYjktZjFkNi00MGVlLTgyZDYtZWYxNjRiNGI1MWYwIiwiZXhwIjoxNzY0MzA3NzEwLCJpc3MiOiJMb2dpc3RpY3NBUEkiLCJhdWQiOiJMb2dpc3RpY3NDbGllbnQifQ.1sZB_ReTB_VUDSzMY1vscd5kfgGr1e6yHwDrrsML6sA"

echo "Creating 30 purchase orders..."

for i in {1..30}; do
    NUM=$(printf "%03d" $i)

    curl -s -X POST http://localhost:5000/api/purchase-orders \
      -H "Authorization: Bearer $TOKEN" \
      -H "Content-Type: application/json" \
      -d "{
        \"companyId\": \"400812ed-61e1-4902-a7d7-1bcf30df3907\",
        \"purchaseOrderNumber\": \"PO-BULK-${NUM}\",
        \"supplierId\": \"12ef831d-4970-4388-a5dd-3a521552b75d\",
        \"expectedDate\": \"2025-12-${NUM:1:2}T10:00:00Z\",
        \"priority\": $((i % 4 + 1)),
        \"items\": [
          {
            \"productId\": \"03ec6d36-1880-4709-9396-3ee9433c8827\",
            \"sku\": \"SKU-00034\",
            \"quantityOrdered\": $((RANDOM % 50 + 10)),
            \"unitPrice\": $((RANDOM % 200 + 50)).00
          },
          {
            \"productId\": \"0421d72c-583b-43b0-8b32-8d8cd9ac949c\",
            \"sku\": \"SKU-00008\",
            \"quantityOrdered\": $((RANDOM % 30 + 5)),
            \"unitPrice\": $((RANDOM % 150 + 25)).00
          }
        ]
      }" | grep -q "success\":true" && echo "✅ Created PO-BULK-${NUM}" || echo "❌ Failed PO-BULK-${NUM}"

    sleep 0.1
done

echo "Bulk creation completed!"