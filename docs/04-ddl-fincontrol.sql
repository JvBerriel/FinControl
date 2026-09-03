-- =============================================================================
-- FinControl — Script DDL PostgreSQL
-- Fase 1 do roadmap: modelagem física a partir do DER (docs/01-der-fincontrol.mermaid)
-- =============================================================================

-- -----------------------------------------------------------------------------
-- Tabela: usuarios
-- -----------------------------------------------------------------------------
CREATE TABLE usuarios (
    id             INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome           VARCHAR(150)   NOT NULL,
    email          VARCHAR(255)   NOT NULL UNIQUE,
    senha_hash     VARCHAR(255)   NOT NULL,
    renda_mensal   DECIMAL(12,2)  NOT NULL DEFAULT 0 CHECK (renda_mensal >= 0),
    criado_em      TIMESTAMPTZ    NOT NULL DEFAULT now()
);

-- -----------------------------------------------------------------------------
-- Tabela: categorias
-- -----------------------------------------------------------------------------
CREATE TABLE categorias (
    id             INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    usuario_id     INTEGER        NOT NULL REFERENCES usuarios(id) ON DELETE CASCADE,
    nome           VARCHAR(100)   NOT NULL,
    cor            VARCHAR(7)     NOT NULL,   -- código hexadecimal, ex: #FF5733
    icone          VARCHAR(50),
    ativa          BOOLEAN        NOT NULL DEFAULT TRUE,

    CONSTRAINT uq_categorias_usuario_nome UNIQUE (usuario_id, nome)
);

CREATE INDEX ix_categorias_usuario_id ON categorias(usuario_id);

-- -----------------------------------------------------------------------------
-- Tabela: transacoes
-- -----------------------------------------------------------------------------
CREATE TABLE transacoes (
    id               INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    usuario_id       INTEGER        NOT NULL REFERENCES usuarios(id) ON DELETE CASCADE,
    categoria_id     INTEGER        NOT NULL REFERENCES categorias(id) ON DELETE RESTRICT,
    descricao        VARCHAR(200)   NOT NULL,
    valor            DECIMAL(12,2)  NOT NULL CHECK (valor > 0),
    tipo             SMALLINT       NOT NULL CHECK (tipo IN (1, 2)), -- 1=Receita 2=Despesa
    data_transacao   DATE           NOT NULL,
    criado_em        TIMESTAMPTZ    NOT NULL DEFAULT now()
);

CREATE INDEX ix_transacoes_usuario_id ON transacoes(usuario_id);
CREATE INDEX ix_transacoes_categoria_id ON transacoes(categoria_id);
CREATE INDEX ix_transacoes_usuario_data ON transacoes(usuario_id, data_transacao);

-- -----------------------------------------------------------------------------
-- Tabela: metas_mensais
-- -----------------------------------------------------------------------------
CREATE TABLE metas_mensais (
    id             INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    usuario_id     INTEGER        NOT NULL REFERENCES usuarios(id) ON DELETE CASCADE,
    categoria_id   INTEGER        NOT NULL REFERENCES categorias(id) ON DELETE RESTRICT,
    valor_limite   DECIMAL(12,2)  NOT NULL CHECK (valor_limite > 0),
    mes            SMALLINT       NOT NULL CHECK (mes BETWEEN 1 AND 12),
    ano            SMALLINT       NOT NULL CHECK (ano >= 2000),

    CONSTRAINT uq_metas_usuario_categoria_periodo UNIQUE (usuario_id, categoria_id, mes, ano)
);

CREATE INDEX ix_metas_mensais_usuario_id ON metas_mensais(usuario_id);
CREATE INDEX ix_metas_mensais_periodo ON metas_mensais(usuario_id, ano, mes);
