# Az function with Key vault

This is a sample project to Connect an Azure function with Key Vault using the App Configuration.

## Prerequisites

It's good having knowledge in:
* [Pulumi](https://www.pulumi.com/docs/intro/languages/dotnet/)
* [Nuke](https://nuke.build/docs/introduction/)

## How to start
First you need to create some required resources like an storage account (for the pulumi state) and a KeyVault to hold your secrets. 

You can use the bash script [CreateRequiredResources.sh](src/AzFunctionsWithKeyVault/Scripts/createRequiredResources.sh) located on the `Scripts` folder. 

After having the required resources created you can go to the [Nuke folder](src/AzFunctionsWithKeyVault/Infrastructure/Base/Nuke/), this folder will contain the necesary pulumi commands to create the `AppConfiguration` with some values linked to Key Vault. After you are located there you can execute the command `dotnet run -target Up` to apply the changes from the code to your Azure account. You will need to supply some parameters like: 
* PULUMI_STATE_STORAGE_ACCOUNT_NAME [The Storage account name created for pulumi]
* PULUMI_STATE_STORAGE_KEY (The shell script will list the access key to the storage acount)
* STAGE -> Dev
* PULUMI_PASSPHRASE -> [Your Secret Passphrase]

With the arguments the command will look like `dotnet run -target Up --PULUMI_STATE_STORAGE_ACCOUNT_NAME [storageName] --PULUMI_STATE_STORAGE_KEY [StorageAccesKey] --STAGE [Stage] --PULUMI_PASSPHRASE [Passphrase]`
 
After Pulumi executed sucesfully, you need to go to the App Configuration resource on azure portal and retrieve the `ConnectionString` on the `Access Keys` section. Copy it and pasted on the `local.settings.json` of your azure function, with the parameter name `AppConfigurationConnectionString`. Then just run the project and when using the function, it will display the variables and secrets located on the App Configuration and linked to key vault.

## How to delete

Rememebr to delete verything or azure will start charging you, for it you can go again to the [Nuke folder](src/AzFunctionsWithKeyVault/Infrastructure/Base/Nuke/) and execute kinda the similar comand, but instead of `Up` you need to change it to `Destroy`. So your command to destroy the resources created by pulumi will be the following:
`dotnet run -target Destroy --PULUMI_STATE_STORAGE_ACCOUNT_NAME [storageName] --PULUMI_STATE_STORAGE_KEY [StorageAccesKey] --STAGE [Stage] --PULUMI_PASSPHRASE [Passphrase]`. After it, execute the bash script [CleanRequiredResources.sh](src/AzFunctionsWithKeyVault/Scripts/cleanRequiredResources.sh) to delete the required resources created in the beginning.
