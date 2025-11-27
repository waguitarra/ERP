#!/bin/bash

# Bulk create purchase orders for testing
# Requires TOKEN environment variable to be set

BASE_URL="http://localhost:5000/api"
COMPANY_ID="400812ed-61e1-4902-a7d7-1bcf30df3907"

# Suppliers
SUPPLIERS=(
    "12ef831d-4970-4388-a5dd-3a521552b75d"
    "14335edf-e34c-43d0-ac89-82c7561cf3c6"
    "1b2f2a81-6ebe-4897-ba3a-434eb33d2b7e"
    "251e3e3a-5dba-41a3-9e00-59be4af8ba83"
    "28e1588a-bac4-4f03-942a-b6e8745adbc1"
)

# Products with SKUs
declare -A PRODUCTS=(
    ["03ec6d36-1880-4709-9396-3ee9433c8827"]="SKU-00034"
    ["0421d72c-583b-43b0-8b32-8d8cd9ac949c"]="SKU-00008"
    ["07f70e0d-3b4e-4e16-a4b9-8e6a08cbf88d"]="SKU-00038"
    ["150a5995-fd49-44da-b335-8579f8151508"]="SKU-00022"
    ["16016f6f-62c8-4ae2-861f-ce8a82f0f70b"]="SKU-00044"
    ["16e888b0-817d-4890-bc77-a5cacd99968f"]="SKU-00015"
    ["1833f136-86fb-4ded-8007-1c98a504e110"]="SKU-00030"
    ["1cc65bad-b6da-4315-93b4-8555b5c7b4e1"]="SKU-00023"
    ["1d360f15-e5a9-420b-b9a4-6677362b3866"]="SKU-00031"
    ["1d42e942-b7de-4ff5-8180-e91810729680"]="SKU-00028"
)

echo "Starting bulk purchase order creation..."

for i in {1..30}; do
    # Random supplier
    SUPPLIER_INDEX=$((RANDOM % ${#SUPPLIERS[@]}))
    SUPPLIER_ID=${SUPPLIERS[$SUPPLIER_INDEX]}

    # Random priority (1-4)
    PRIORITY=$((RANDOM % 4 + 1))

    # Random expected date (1-30 days from now)
    DAYS_AHEAD=$((RANDOM % 30 + 1))
    EXPECTED_DATE=$(date -d "+${DAYS_AHEAD} days" +%Y-%m-%dT%H:%M:%S)

    # Random number of items (1-5)
    NUM_ITEMS=$((RANDOM % 5 + 1))

    # Build items array
    ITEMS=""
    PRODUCT_KEYS=(${!PRODUCTS[@]})
    for j in $(seq 1 $NUM_ITEMS); do
        # Random product
        PRODUCT_INDEX=$((RANDOM % ${#PRODUCT_KEYS[@]}))
        PRODUCT_ID=${PRODUCT_KEYS[$PRODUCT_INDEX]}
        SKU=${PRODUCTS[$PRODUCT_ID]}

        # Random quantity (1-100) and price (10-500)
        QUANTITY=$((RANDOM % 100 + 1))
        PRICE=$((RANDOM % 490 + 10))

        ITEMS="${ITEMS}{\"productId\":\"${PRODUCT_ID}\",\"sku\":\"${SKU}\",\"quantityOrdered\":${QUANTITY},\"unitPrice\":${PRICE}.00},"
    done
    # Remove trailing comma
    ITEMS=${ITEMS%,}

    # Create purchase order
    RESPONSE=$(curl -s -X POST "${BASE_URL}/purchase-orders" \
        -H "Authorization: Bearer ${TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"companyId\": \"${COMPANY_ID}\",
            \"purchaseOrderNumber\": \"PO-BULK-$(printf "%03d" $i)\",
            \"supplierId\": \"${SUPPLIER_ID}\",
            \"expectedDate\": \"${EXPECTED_DATE}\",
            \"priority\": ${PRIORITY},
            \"items\": [${ITEMS}]
        }")

    if [[ $RESPONSE == *"success\":true"* ]]; then
        echo "✅ Created PO-BULK-$(printf "%03d" $i)"
    else
        echo "❌ Failed to create PO-BULK-$(printf "%03d" $i): $RESPONSE"
    fi

    # Small delay to avoid overwhelming the API
    sleep 0.1
done

echo "Bulk creation completed!"