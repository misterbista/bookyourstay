FROM mcr.microsoft.com/dotnet/sdk:10.0

ARG BUN_VERSION=1.3.4
ARG USERNAME=dev
ARG USER_UID=1000
ARG USER_GID=1000

RUN apt-get update \
    && apt-get install -y --no-install-recommends bash ca-certificates curl git unzip \
    && rm -rf /var/lib/apt/lists/*

RUN set -eux; \
    group_name="${USERNAME}"; \
    if getent group "${USER_GID}" > /dev/null; then \
        group_name="$(getent group "${USER_GID}" | cut -d: -f1)"; \
    else \
        groupadd --gid "${USER_GID}" "${group_name}"; \
    fi; \
    if id -u "${USERNAME}" > /dev/null 2>&1; then \
        usermod --uid "${USER_UID}" --gid "${USER_GID}" "${USERNAME}"; \
    elif getent passwd "${USER_UID}" > /dev/null; then \
        existing_user="$(getent passwd "${USER_UID}" | cut -d: -f1)"; \
        usermod --login "${USERNAME}" "${existing_user}"; \
        usermod --home "/home/${USERNAME}" --move-home "${USERNAME}"; \
        usermod --gid "${USER_GID}" "${USERNAME}"; \
    else \
        useradd --uid "${USER_UID}" --gid "${group_name}" --create-home --shell /bin/bash "${USERNAME}"; \
    fi

USER ${USERNAME}

ENV BUN_INSTALL=/home/${USERNAME}/.bun
ENV PATH=${BUN_INSTALL}/bin:/home/${USERNAME}/.dotnet/tools:${PATH}

RUN curl -fsSL https://bun.sh/install | bash -s -- bun-v${BUN_VERSION}

WORKDIR /workspace

CMD ["sleep", "infinity"]
