#!/bin/bash

# Variables
RESOURCE_GROUP="ayoubjabirfa"
LOCATION="northeurope"
STORAGE_ACCOUNT="ayoubjabirstorage"
CONTAINER_INPUT="input"
CONTAINER_OUTPUT="output"
LOGIC_APP_NAME="logicapp-ayoubjabir"

echo "📦 Création du Storage Account : $STORAGE_ACCOUNT ..."
az storage account create --name $STORAGE_ACCOUNT --location $LOCATION --resource-group $RESOURCE_GROUP --sku Standard_LRS

echo "🔑 Récupération de la clé de stockage ..."
STORAGE_KEY=$(az storage account keys list --resource-group $RESOURCE_GROUP --account-name $STORAGE_ACCOUNT --query "[0].value" -o tsv)

echo "📁 Création des containers blob : $CONTAINER_INPUT, $CONTAINER_OUTPUT ..."
az storage container create --name $CONTAINER_INPUT --account-name $STORAGE_ACCOUNT --account-key $STORAGE_KEY
az storage container create --name $CONTAINER_OUTPUT --account-name $STORAGE_ACCOUNT --account-key $STORAGE_KEY

echo "⚙️  Création de la Logic App : $LOGIC_APP_NAME ..."
az logic workflow create --resource-group $RESOURCE_GROUP --location $LOCATION --name $LOGIC_APP_NAME --definition "{}"

echo "✅ Tous les composants sont créés ! Tu peux maintenant aller sur https://portal.azure.com et configurer la Logic App : $LOGIC_APP_NAME"
