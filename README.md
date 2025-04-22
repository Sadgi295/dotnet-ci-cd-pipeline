# DevOps Technical Task - .NET Counter Service CI/CD

## Overview

This repository contains a simple .NET service (`/count` endpoint) and the Azure Pipelines configuration to automatically build, test, and deploy it. The goal is to demonstrate a basic, automated CI/CD workflow.

## CI/CD Setup with Azure Pipelines

We use Azure Pipelines to automate the integration and deployment process. The setup consists of two separate pipelines as requested:

1.  **CI Pipeline (`azure-build.yml`)**: Handles Continuous Integration.
2.  **CD Pipeline (`azure-release.yml`)**: Handles Continuous Deployment.

### 1. CI Pipeline (`azure-build.yml`)

This pipeline focuses on building and validating the code.

* **Triggers**:
    * Automatically runs on every push to the `main` branch.
    * Automatically runs for Pull Requests targeting the `main` branch.
    * Uses `batch: true` to group multiple quick commits into a single run.
    * Path filters ensure the pipeline runs only when relevant code (`.cs`, `.csproj`, pipeline files) changes.
* **Agent**: Runs on an `ubuntu-latest` hosted agent.
* **Steps**:
    1.  **Setup .NET SDK**: Ensures the correct .NET SDK version (defined in variables) is available.
    2.  **Restore Dependencies**: Runs `dotnet restore` to fetch NuGet packages.
    3.  **Build Project**: Runs `dotnet build` in `Release` configuration.
    4.  **Run Tests**: Executes `dotnet test` against the test project and publishes results to Azure Pipelines for review.
    5.  **Publish Project**: Runs `dotnet publish` to create a deployable package (`.zip` file) for the web application.
    6.  **Publish Artifact**: Uploads the zipped application package (`drop` artifact) to Azure Pipelines, making it available for the CD pipeline.

### 2. CD Pipeline (`azure-release.yml`)

This pipeline focuses on deploying the validated application artifact to an environment.

* **Trigger**:
    * Configured in Azure DevOps to automatically trigger upon the successful completion of the CI pipeline (`azure-build.yml`) for the `main` branch. The `resources` block in the YAML shows how this dependency can be declared.
* **Agent**: Runs on an `ubuntu-latest` hosted agent.
* **Deployment Target**: Azure App Service (Linux or Windows, configured in the task). Assumes an App Service named `my-dotnet-counter-app` and an Azure Service Connection named `azure-connection` exist.
* **Steps**:
    1.  **Download Artifact**: Downloads the `drop` artifact produced by the triggering CI pipeline run.
    2.  **Deploy to Azure App Service**: Uses the `AzureWebApp@1` task to deploy the downloaded `.zip` package to the specified Azure App Service.

### Design Decisions

* **Separate CI and CD Pipelines**: As per requirements, CI and CD are implemented in two distinct Azure Pipelines (`azure-build.yml` for CI, `azure-release.yml` definition for CD). This provides a clear separation of concerns. While multi-stage pipelines within a single YAML file are also common, this approach fulfills the specific requirement.
* **Artifact Publishing**: The CI pipeline publishes a `.zip` artifact. This is a straightforward way to package the .NET application for deployment to Azure App Service, especially for the Free Tier which might have limitations regarding container registries. Docker was an option, but artifact deployment is simpler for this scenario.
* **YAML Pipelines**: Both pipelines are defined using YAML, aligning with modern Azure DevOps practices for pipeline-as-code.
* **Agent Choice**: `ubuntu-latest` is chosen as it's generally faster and more cost-effective for .NET Core applications, which are cross-platform.
* **Variables**: Key settings like build configuration and .NET SDK version are defined as variables for easy modification.

### Best Practices Implementation

* **PR Validation**: The CI pipeline (`azure-build.yml`) includes a `pr` trigger for the `main` branch. This ensures that code is tested and builds successfully *before* it gets merged into `main`, catching issues early. Combine this with Branch Policies in Azure Repos or GitHub (require successful build, code reviews) for robust quality control.
* **CI Trigger (Post-Merge)**: The `trigger` section ensures that once code is merged to `main`, a build runs, producing a deployable artifact.
* **CD Trigger (Post-CI)**: The CD pipeline is triggered only after a successful CI build on the `main` branch, ensuring only validated code is deployed.
* **Separation**: Keeping CI and CD separate allows for independent management and potentially different permissions or approval processes for deployment.
* **Approval Gates (Recommendation)**: For real-world scenarios, especially deploying to Staging or Production environments, add approval gates.
    * **YAML Pipelines**: Use `environment` definitions in Azure DevOps and configure approvals on the environment. The `environment: 'production'` line in `azure-release.yml` facilitates this.
    * **Classic Release Pipelines**: If using the classic UI, approvals can be configured on stages directly.
* **Secrets Management**: The Azure Subscription connection (`azure-connection`) should be configured securely in Azure DevOps Project Settings -> Service Connections. Avoid hardcoding secrets.

### Setup Instructions

1.  **Repository**: Push code, including `azure-build.yml` and `azure-release.yml`, to a GitHub or Azure Repos repository.
2.  **Azure DevOps Project**: Create an Azure DevOps project.
3.  **Azure App Service**: Create an Azure App Service instance (Free Tier is suitable). Note its name (e.g., `my-dotnet-counter-app`) and whether it's Linux or Windows.
4.  **Azure Service Connection**: In Azure DevOps Project Settings -> Service Connections, create an "Azure Resource Manager" service connection pointing to Azure subscription. Grant it appropriate permissions (e.g., Contributor) on the App Service or its Resource Group. Name it `azure-connection` (or update the name in `azure-release.yml`).
5.  **Create CI Pipeline**: In Azure DevOps Pipelines, create a "New Pipeline". Point it to repository and select "Existing Azure Pipelines YAML file". Choose `/azure-build.yml`. Save and run.
6.  **Create CD Pipeline**: In Azure DevOps Pipelines, create another "New Pipeline". Point it to repository and select "Existing Azure Pipelines YAML file". Choose `/azure-release.yml`.
    * **Crucially**: After creating the CD pipeline, go to its settings (Edit -> Triggers -> YAML -> Get sources or Resources) and ensure it's configured to trigger automatically after the CI pipeline (BuildPipelineName`) completes for the `main` branch. might need to manually link the build artifact resource if the `resources` block isn't automatically picked up or if prefer UI configuration for triggers.
7.  **Branch Policies (Recommended)**: In repository settings (GitHub or Azure Repos), configure branch policies for `main`:
    * Require pull requests.
    * Require successful build validation (link CI pipeline).
    * Require code reviews (optional but recommended).

Now, when push to `main` or create a PR, the CI pipeline will run. If the CI for `main` succeeds, the CD pipeline will automatically trigger and deploy to Azure App Service.
