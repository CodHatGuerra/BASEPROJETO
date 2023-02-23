SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ACAO](
	[cd_Acao] [int] NOT NULL,
	[ds_Acao] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[cd_Acao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CATEGORIA]    Script Date: 28/09/2016 20:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[CATEGORIA](
	[cd_Categoria] [int] IDENTITY(1,1) NOT NULL,
	[ds_Categoria] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[cd_Categoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FORNECEDOR]    Script Date: 28/09/2016 20:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[FORNECEDOR](
	[cd_Fornecedor] [int] IDENTITY(1,1) NOT NULL,
	[nm_Fornecedor] [varchar](200) NOT NULL,
	[ds_Endereco] [varchar](200) NULL,
	[ds_Numero] [varchar](20) NULL,
	[ds_Bairro] [varchar](50) NULL,
	[cd_Cidade] [int] NULL,
	[ds_Cpf] [varchar](30) NULL,
	[ds_Rg] [varchar](30) NULL,
	[nm_Fantasia] [varchar](200) NULL,
 CONSTRAINT [CD_FORNECEDOR_PK] PRIMARY KEY CLUSTERED 
(
	[cd_Fornecedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MODULO]    Script Date: 28/09/2016 20:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MODULO](
	[cd_Modulo] [int] NOT NULL,
	[ds_Modulo] [varchar](200) NULL,
	[cd_ModuloPai] [int] NULL,
	[ds_OrdemExibicao] [int] NULL,
	[ds_Icon] [varchar](200) NULL,
	[Ativo] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[cd_Modulo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MODULO_ACAO]    Script Date: 28/09/2016 20:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MODULO_ACAO](
	[cd_PerfilModulo] [int] NOT NULL,
	[cd_Acao] [int] NOT NULL,
 CONSTRAINT [MODULO_ACAO_PK] PRIMARY KEY CLUSTERED 
(
	[cd_PerfilModulo] ASC,
	[cd_Acao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PERFIL_MODULO]    Script Date: 28/09/2016 20:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PERFIL_MODULO](
	[cd_PerfilModulo] [int] IDENTITY(1,1) NOT NULL,
	[cd_Perfil] [int] NOT NULL,
	[cd_Modulo] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[cd_PerfilModulo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PERFIL_USUARIO]    Script Date: 28/09/2016 20:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PERFIL_USUARIO](
	[cd_PerfilUsuario] [int] IDENTITY(1,1) NOT NULL,
	[cd_Perfil] [int] NOT NULL,
	[cd_Usuario] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[cd_PerfilUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UNIDADEMEDIDA]    Script Date: 28/09/2016 20:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UNIDADEMEDIDA](
	[cd_UnidadeMedida] [int] IDENTITY(1,1) NOT NULL,
	[ds_UnidadeMedida] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[cd_UnidadeMedida] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[ACAO] ([cd_Acao], [ds_Acao]) VALUES (1, N'Leitura')
GO
INSERT [dbo].[ACAO] ([cd_Acao], [ds_Acao]) VALUES (2, N'Inserir')
GO
INSERT [dbo].[ACAO] ([cd_Acao], [ds_Acao]) VALUES (3, N'Editar')
GO
INSERT [dbo].[ACAO] ([cd_Acao], [ds_Acao]) VALUES (4, N'Remover')
GO
SET IDENTITY_INSERT [dbo].[CATEGORIA] ON 

GO
INSERT [dbo].[CATEGORIA] ([cd_Categoria], [ds_Categoria]) VALUES (1, N'Não Especificado')
GO
SET IDENTITY_INSERT [dbo].[CATEGORIA] OFF
GO
SET IDENTITY_INSERT [dbo].[FORNECEDOR] ON 

GO
INSERT [dbo].[FORNECEDOR] ([cd_Fornecedor], [nm_Fornecedor], [ds_Endereco], [ds_Numero], [ds_Bairro], [cd_Cidade], [ds_Cpf], [ds_Rg], [nm_Fantasia]) VALUES (1, N'Biguá', N'Av Pedro Ometto', N'30', N'Centro', 1, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[FORNECEDOR] OFF
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (1, N'INÍCIO', NULL, 1, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (100, N'CADASTROS', NULL, 2, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (101, N'Cidade', 100, 1, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (102, N'Cliente', 100, 2, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (103, N'Fornecedor', 100, 3, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (104, N'Funcionário', 100, 4, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (105, N'Produto', 100, 5, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (106, N'Usuário', 100, 6, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (107, N'Costureira', 100, 7, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (108, N'Gola', 100, 8, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (109, N'Malha', 100, 9, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (110, N'Tamanho', 100, 10, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (200, N'VENDAS', NULL, 3, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (201, N'Venda', 200, 1, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (202, N'Controle Costureira', 200, 2, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (203, N'Controle Entrega', 200, 3, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (300, N'PAGAMENTOS', NULL, 4, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (301, N'Controle do Mês', 300, 1, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (302, N'Pagamento Costureira', 300, 2, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (400, N'RELATÓRIOS', NULL, 5, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (401, N'Estatísticas de Produtos', 400, 1, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (402, N'Estatísticas de Vendas', 400, 2, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (403, N'Pagamentos', 400, 3, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (404, N'Pendências Pagamentos', 400, 4, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (405, N'Venda', 400, 5, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (406, N'Costureira', 400, 6, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (500, N'OPÇÕES', NULL, 6, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (501, N'Logs', 500, 1, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (502, N'Trocar Usuário', 500, 2, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (503, N'Sair da Aplicação', 500, 3, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (600, N'AJUDA', NULL, 7, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (601, N'Manual', 600, 1, 1)
GO
INSERT [dbo].[MODULO] ([cd_Modulo], [ds_Modulo], [cd_ModuloPai], [ds_OrdemExibicao], [Ativo]) VALUES (602, N'Sobre', 600, 2, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (3, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (3, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (3, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (3, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (4, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (4, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (4, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (4, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (5, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (5, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (5, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (5, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (6, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (6, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (6, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (6, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (7, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (7, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (7, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (7, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (8, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (8, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (8, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (8, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (10, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (10, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (10, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (10, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (11, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (11, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (11, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (11, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (12, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (12, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (12, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (12, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (14, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (14, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (14, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (14, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (15, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (15, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (15, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (15, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (17, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (17, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (17, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (17, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (18, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (18, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (18, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (18, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (19, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (19, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (19, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (19, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (20, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (20, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (20, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (20, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (21, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (21, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (21, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (21, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (23, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (23, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (23, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (23, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (24, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (24, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (24, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (24, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (25, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (25, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (25, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (25, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (27, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (27, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (27, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (27, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (28, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (28, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (28, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (28, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (29, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (29, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (29, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (29, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (30, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (30, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (30, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (30, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (31, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (31, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (31, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (31, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (32, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (32, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (32, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (32, 4)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (33, 1)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (33, 2)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (33, 3)
GO
INSERT [dbo].[MODULO_ACAO] ([cd_PerfilModulo], [cd_Acao]) VALUES (33, 4)
GO
SET IDENTITY_INSERT [dbo].[PERFIL_MODULO] ON 

GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (1, 1, 1)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (2, 1, 100)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (3, 1, 101)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (4, 1, 102)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (5, 1, 103)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (6, 1, 104)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (7, 1, 105)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (8, 1, 106)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (9, 1, 200)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (10, 1, 201)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (11, 1, 202)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (12, 1, 203)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (13, 1, 300)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (14, 1, 301)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (15, 1, 302)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (16, 1, 400)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (17, 1, 401)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (18, 1, 402)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (19, 1, 403)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (20, 1, 404)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (21, 1, 405)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (22, 1, 500)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (23, 1, 501)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (24, 1, 502)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (25, 1, 503)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (26, 1, 600)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (27, 1, 601)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (28, 1, 602)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (29, 1, 107)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (30, 1, 108)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (31, 1, 109)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (32, 1, 110)
GO
INSERT [dbo].[PERFIL_MODULO] ([cd_PerfilModulo], [cd_Perfil], [cd_Modulo]) VALUES (33, 1, 406)
GO
SET IDENTITY_INSERT [dbo].[PERFIL_MODULO] OFF
GO
SET IDENTITY_INSERT [dbo].[PERFIL_USUARIO] ON 

GO
INSERT [dbo].[PERFIL_USUARIO] ([cd_PerfilUsuario], [cd_Perfil], [cd_Usuario]) VALUES (1, 1, 16)
GO
INSERT [dbo].[PERFIL_USUARIO] ([cd_PerfilUsuario], [cd_Perfil], [cd_Usuario]) VALUES (2, 2, 18)
GO
GO
SET IDENTITY_INSERT [dbo].[PERFIL_USUARIO] OFF
GO
SET IDENTITY_INSERT [dbo].[UNIDADEMEDIDA] ON 

GO
INSERT [dbo].[UNIDADEMEDIDA] ([cd_UnidadeMedida], [ds_UnidadeMedida]) VALUES (1, N'Un')
GO
INSERT [dbo].[UNIDADEMEDIDA] ([cd_UnidadeMedida], [ds_UnidadeMedida]) VALUES (2, N'Kg')
GO
INSERT [dbo].[UNIDADEMEDIDA] ([cd_UnidadeMedida], [ds_UnidadeMedida]) VALUES (3, N'Mt')
GO
INSERT [dbo].[UNIDADEMEDIDA] ([cd_UnidadeMedida], [ds_UnidadeMedida]) VALUES (4, N'L')
GO
SET IDENTITY_INSERT [dbo].[UNIDADEMEDIDA] OFF
GO
ALTER TABLE [dbo].[FORNECEDOR]  WITH CHECK ADD  CONSTRAINT [cidade_Fornecedor_FK] FOREIGN KEY([cd_Cidade])
REFERENCES [dbo].[CIDADE] ([cd_Cidade])
GO
ALTER TABLE [dbo].[FORNECEDOR] CHECK CONSTRAINT [cidade_Fornecedor_FK]
GO
ALTER TABLE [dbo].[MODULO]  WITH CHECK ADD  CONSTRAINT [Modulo_Modulo_FK] FOREIGN KEY([cd_ModuloPai])
REFERENCES [dbo].[MODULO] ([cd_Modulo])
GO
ALTER TABLE [dbo].[MODULO] CHECK CONSTRAINT [Modulo_Modulo_FK]
GO
ALTER TABLE [dbo].[MODULO_ACAO]  WITH CHECK ADD  CONSTRAINT [MA_aCAO_FK] FOREIGN KEY([cd_Acao])
REFERENCES [dbo].[ACAO] ([cd_Acao])
GO
ALTER TABLE [dbo].[MODULO_ACAO] CHECK CONSTRAINT [MA_aCAO_FK]
GO
ALTER TABLE [dbo].[MODULO_ACAO]  WITH CHECK ADD  CONSTRAINT [MA_PMODULO_FK] FOREIGN KEY([cd_PerfilModulo])
REFERENCES [dbo].[PERFIL_MODULO] ([cd_PerfilModulo])
GO
ALTER TABLE [dbo].[MODULO_ACAO] CHECK CONSTRAINT [MA_PMODULO_FK]
GO
ALTER TABLE [dbo].[PERFIL_MODULO]  WITH CHECK ADD  CONSTRAINT [Modulo_Perfil_FK] FOREIGN KEY([cd_Modulo])
REFERENCES [dbo].[MODULO] ([cd_Modulo])
GO
ALTER TABLE [dbo].[PERFIL_MODULO] CHECK CONSTRAINT [Modulo_Perfil_FK]
GO
ALTER TABLE [dbo].[PERFIL_MODULO]  WITH CHECK ADD  CONSTRAINT [Perfil_Modulo_FK] FOREIGN KEY([cd_Perfil])
REFERENCES [dbo].[PERFIL] ([cd_Perfil])
GO
ALTER TABLE [dbo].[PERFIL_MODULO] CHECK CONSTRAINT [Perfil_Modulo_FK]
GO
ALTER TABLE [dbo].[PERFIL_USUARIO]  WITH CHECK ADD  CONSTRAINT [PU_Perfil_FK] FOREIGN KEY([cd_Perfil])
REFERENCES [dbo].[PERFIL] ([cd_Perfil])
GO
ALTER TABLE [dbo].[PERFIL_USUARIO] CHECK CONSTRAINT [PU_Perfil_FK]
GO
ALTER TABLE [dbo].[PERFIL_USUARIO]  WITH CHECK ADD  CONSTRAINT [PU_Usuario_FK] FOREIGN KEY([cd_Usuario])
REFERENCES [dbo].[LOGIN] ([cd_Login])
GO
ALTER TABLE [dbo].[PERFIL_USUARIO] CHECK CONSTRAINT [PU_Usuario_FK]
GO

/****** Object:  Table [dbo].[CONTATOFORNECEDOR]    Script Date: 28/09/2016 21:14:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[CONTATOFORNECEDOR](
	[cd_ContatoFornecedor] [int] IDENTITY(1,1) NOT NULL,
	[cd_Fornecedor] [int] NOT NULL,
	[ds_Contato] [varchar](50) NOT NULL,
	[cd_TipoContato] [int] NOT NULL,
 CONSTRAINT [PK_CONTATOFORNECEDOR] PRIMARY KEY CLUSTERED 
(
	[cd_ContatoFornecedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[CONTATOFORNECEDOR] ON 

GO
INSERT [dbo].[CONTATOFORNECEDOR] ([cd_ContatoFornecedor], [cd_Fornecedor], [ds_Contato], [cd_TipoContato]) VALUES (2, 1, N'14 991046875', 1)
GO
SET IDENTITY_INSERT [dbo].[CONTATOFORNECEDOR] OFF
GO
ALTER TABLE [dbo].[CONTATOFORNECEDOR]  WITH CHECK ADD  CONSTRAINT [cd_TipoContato_Fornecedor_FK] FOREIGN KEY([cd_TipoContato])
REFERENCES [dbo].[TIPOCONTATO] ([cd_TipoContato])
GO
ALTER TABLE [dbo].[CONTATOFORNECEDOR] CHECK CONSTRAINT [cd_TipoContato_Fornecedor_FK]
GO
ALTER TABLE [dbo].[CONTATOFORNECEDOR]  WITH CHECK ADD  CONSTRAINT [CONTATOFORNECEDOR_FORNECEDOR_FK] FOREIGN KEY([cd_Fornecedor])
REFERENCES [dbo].[FORNECEDOR] ([cd_Fornecedor])
GO
ALTER TABLE [dbo].[CONTATOFORNECEDOR] CHECK CONSTRAINT [CONTATOFORNECEDOR_FORNECEDOR_FK]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ProcRecuperarModuloAcao')
DROP PROCEDURE ProcRecuperarModuloAcao
GO

CREATE PROCEDURE ProcRecuperarModuloAcao
(
  @CodigoUsuario INT,
  @CodigoModulo  INT  
)
AS 
BEGIN
  
  SELECT DISTINCT MA.cd_Acao 
    FROM MODULO_ACAO MA INNER JOIN
         PERFIL_MODULO PF ON MA.cd_PerfilModulo = PF.cd_PerfilModulo INNER JOIN
  	     PERFIL_USUARIO PU ON PF.cd_Perfil = PU.cd_Perfil
   WHERE PU.cd_Usuario = @CodigoUsuario
     AND PF.cd_Modulo = @CodigoModulo

END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ProcRecuperarModulo')
DROP PROCEDURE ProcRecuperarModulo
GO

CREATE PROCEDURE ProcRecuperarModulo
(
  @CodigoUsuario INT 
)
AS 
BEGIN
  
  SELECT DISTINCT M.cd_Modulo, 
         M.ds_Modulo,
		 M.cd_ModuloPai,
		 M.ds_OrdemExibicao,
		 M.ds_Icon
    FROM MODULO M INNER JOIN
         PERFIL_MODULO PF ON M.cd_Modulo = PF.cd_Modulo INNER JOIN
  	     PERFIL_USUARIO PU ON PF.cd_Perfil = PU.cd_Perfil
   WHERE PU.cd_Usuario = @CodigoUsuario
     AND M.Ativo = 1
	 AND EXISTS(SELECT 1 FROM MODULO_ACAO WHERE cd_PerfilModulo = PF.cd_PerfilModulo AND cd_Acao = 1)
END
GO


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ProcRecuperarPerfilModuloAcao')
DROP PROCEDURE ProcRecuperarPerfilModuloAcao
GO

CREATE PROCEDURE ProcRecuperarPerfilModuloAcao
(
  @CodigoPerfil INT
)
AS 
BEGIN
  
 SELECT M.cd_Modulo,
        M.ds_Modulo,
		M.cd_ModuloPai,
		M.ds_OrdemExibicao,
        CASE WHEN EXISTS(SELECT 1 FROM MODULO_ACAO MA WHERE MA.cd_PerfilModulo = PM.cd_PerfilModulo AND MA.cd_Acao = 1) THEN 1 ELSE 0 END AS leitura,
        CASE WHEN EXISTS(SELECT 1 FROM MODULO_ACAO MA WHERE MA.cd_PerfilModulo = PM.cd_PerfilModulo AND MA.cd_Acao = 2) THEN 1 ELSE 0 END AS inserir,
	    CASE WHEN EXISTS(SELECT 1 FROM MODULO_ACAO MA WHERE MA.cd_PerfilModulo = PM.cd_PerfilModulo AND MA.cd_Acao = 3) THEN 1 ELSE 0 END AS editar,
	    CASE WHEN EXISTS(SELECT 1 FROM MODULO_ACAO MA WHERE MA.cd_PerfilModulo = PM.cd_PerfilModulo AND MA.cd_Acao = 4) THEN 1 ELSE 0 END AS remover
   FROM MODULO M LEFT JOIN
        PERFIL_MODULO PM ON M.cd_Modulo = PM.cd_Modulo
    AND PM.cd_Perfil = COALESCE(@CodigoPerfil,PM.cd_Perfil)

END
GO

IF NOT EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PRODUTOS' AND  COLUMN_NAME = 'ds_PrecoCusto') BEGIN
   ALTER TABLE PRODUTOS
     ADD ds_PrecoCusto    NUMERIC(10,4),
	     ds_PrecoVenda    NUMERIC(10,4),
		 ds_EstoqueMinimo INT,
		 ds_EstoqueAtual  INT,
		 cd_UnidadeMedida INT,
		 cd_Fornecedor    INT,
		 cd_Categoria     INT,
		 CONSTRAINT P_UnidadeMedida_FK FOREIGN KEY (cd_UnidadeMedida) REFERENCES UNIDADEMEDIDA (cd_UnidadeMedida),
		 CONSTRAINT P_Fornecedor_FK FOREIGN KEY (cd_Fornecedor) REFERENCES FORNECEDOR (cd_Fornecedor),
		 CONSTRAINT P_Categoria_FK FOREIGN KEY (cd_Categoria) REFERENCES CATEGORIA (cd_Categoria)
END
GO