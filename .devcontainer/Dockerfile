# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.187.0/containers/dotnet/.devcontainer/base.Dockerfile

# [Choice] .NET version: 5.0, 3.1, 2.1
ARG VARIANT="5.0"
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-${VARIANT}

# [Option] Install Node.js
ARG INSTALL_NODE="true"
ARG NODE_VERSION="lts/*"
RUN if [ "${INSTALL_NODE}" = "true" ]; then su vscode -c "umask 0002 && . /usr/local/share/nvm/nvm.sh && nvm install ${NODE_VERSION} 2>&1"; fi

ARG INSTALL_AZURE_CLI="true"
ARG INSTALL_POWERSHELL_CORE="true"
ARG INSTALL_DOCKER_CLI="true"

COPY library-scripts/*.sh library-scripts/*.env /tmp/library-scripts/
RUN if [ "$INSTALL_AZURE_CLI" = "true" ]; then bash /tmp/library-scripts/azcli-debian.sh; fi \
    && apt-get clean -y && rm -rf /var/lib/apt/lists/* /tmp/library-scripts

RUN if [ "${INSTALL_POWERSHELL_CORE}" = "true" ]; then \
        wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb \
        && dpkg -i packages-microsoft-prod.deb \
        && apt-get update \
        && apt-get install -y powershell; \
    fi

RUN if [ "${INSTALL_DOCKER_CLI}" = "true" ]; then \
        apt-get update \
        && apt-get install -y apt-transport-https ca-certificates curl gnupg-agent software-properties-common lsb-release \
        && curl -fsSL https://download.docker.com/linux/$(lsb_release -is | tr '[:upper:]' '[:lower:]')/gpg | apt-key add - 2>/dev/null \
        && add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/$(lsb_release -is | tr '[:upper:]' '[:lower:]') $(lsb_release -cs) stable" \
        && apt-get update \
        && apt-get install -y docker-ce-cli; \
    fi