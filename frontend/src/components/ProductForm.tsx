import { type FormEvent, useEffect, useState } from "react";

type ProductFormData = {
  category: string;
  sku: string;
  name: string;
  description: string;
  price: string;
  stock: string;
};

type Product = {
  id: number;
  sku: string;
  name: string;
  category: string;
  description: string;
  price: number;
  stock: number;
};

const initialData: ProductFormData = {
  category: "",
  sku: "",
  name: "",
  description: "",
  price: "",
  stock: "",
};

function formatPriceInput(value: string): string {
  const digits = value.replace(/\D/g, "");
  if (!digits) return "";

  const amount = Number(digits) / 100;
  return amount.toLocaleString("pt-BR", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
}

function parsePriceInput(value: string): number {
  const normalized = value.replace(/\./g, "").replace(",", ".").trim();
  return Number(normalized);
}

function formatPriceDisplay(value: number): string {
  return value.toLocaleString("pt-BR", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
}

export default function ProductForm() {
  const [products, setProducts] = useState<Product[]>([]);
  const [formData, setFormData] = useState<ProductFormData>(initialData);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [errorMessage, setErrorMessage] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);

  useEffect(() => {
    void loadProducts();
  }, []);

  async function loadProducts() {
    setLoading(true);
    setErrorMessage("");

    try {
      const response = await fetch("/product");
      if (!response.ok) {
        throw new Error("Não foi possível carregar os produtos.");
      }

      const data = (await response.json()) as Product[];
      setProducts(data);
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "Erro ao carregar produtos.";
      setErrorMessage(message);
    } finally {
      setLoading(false);
    }
  }

  function handleChange(field: keyof ProductFormData, value: string) {
    setFormData((prev) => ({ ...prev, [field]: value }));
  }

  function resetForm() {
    setFormData(initialData);
    setEditingId(null);
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setErrorMessage("");

    const payload = {
      category: formData.category.trim(),
      sku: formData.sku.trim(),
      name: formData.name.trim(),
      description: formData.description.trim(),
      price: parsePriceInput(formData.price),
      stock: Number(formData.stock),
    };

    if (Number.isNaN(payload.price)) {
      setErrorMessage("Preço inválido.");
      return;
    }

    const url = editingId ? `/product/${editingId}` : "/product";
    const method = editingId ? "PUT" : "POST";

    try {
      const response = await fetch(url, {
        method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const data = (await response.json().catch(() => null)) as {
          message?: string;
        } | null;
        throw new Error(data?.message ?? "Não foi possível salvar o produto.");
      }

      await loadProducts();
      resetForm();
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "Erro ao salvar produto.";
      setErrorMessage(message);
    }
  }

  function handleEdit(product: Product) {
    setEditingId(product.id);
    setErrorMessage("");
    setFormData({
      category: product.category,
      sku: product.sku,
      name: product.name,
      description: product.description,
      price: formatPriceDisplay(product.price),
      stock: String(product.stock),
    });
  }

  async function handleDelete(id: number) {
    setErrorMessage("");

    try {
      const response = await fetch(`/product/${id}`, { method: "DELETE" });
      if (!response.ok) {
        const data = (await response.json().catch(() => null)) as {
          message?: string;
        } | null;
        throw new Error(data?.message ?? "Não foi possível remover o produto.");
      }

      if (editingId === id) {
        resetForm();
      }

      await loadProducts();
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "Erro ao remover produto.";
      setErrorMessage(message);
    }
  }

  return (
    <section className="card">
      <h1>Produtos</h1>
      <p className="subtitle">Cadastro e listagem consumindo a API.</p>

      {errorMessage && <p className="error-message">{errorMessage}</p>}

      <form className="product-form" onSubmit={handleSubmit}>
        <label htmlFor="category">Categoria</label>
        <input
          id="category"
          type="text"
          value={formData.category}
          onChange={(event) => handleChange("category", event.target.value)}
          maxLength={50}
          required
        />

        <label htmlFor="sku">SKU</label>
        <input
          id="sku"
          type="text"
          value={formData.sku}
          onChange={(event) => handleChange("sku", event.target.value)}
          maxLength={50}
          required
        />

        <label htmlFor="name">Nome</label>
        <input
          id="name"
          type="text"
          value={formData.name}
          onChange={(event) => handleChange("name", event.target.value)}
          maxLength={100}
          required
        />

        <label htmlFor="description">Descrição</label>
        <textarea
          id="description"
          value={formData.description}
          onChange={(event) => handleChange("description", event.target.value)}
          maxLength={500}
          rows={4}
          required
        />

        <label htmlFor="price">Preço</label>
        <div className="price-field">
          <span>R$</span>
          <input
            id="price"
            type="text"
            inputMode="numeric"
            value={formData.price}
            onChange={(event) =>
              handleChange("price", formatPriceInput(event.target.value))
            }
            placeholder="0,00"
            required
          />
        </div>

        <label htmlFor="stock">Estoque</label>
        <input
          id="stock"
          type="number"
          value={formData.stock}
          onChange={(event) => handleChange("stock", event.target.value)}
          min="0"
          step="1"
          required
        />

        <div className="form-actions">
          <button type="submit">{editingId ? "Salvar" : "Adicionar"}</button>
          {editingId && (
            <button type="button" className="secondary" onClick={resetForm}>
              Cancelar
            </button>
          )}
        </div>
      </form>

      <h2>Lista de Produtos</h2>
      {loading ? (
        <p>Carregando...</p>
      ) : products.length === 0 ? (
        <p>Nenhum produto cadastrado.</p>
      ) : (
        <ul className="product-list">
          {products.map((product) => (
            <li key={product.id}>
              <div>
                <strong>{product.name}</strong> ({product.sku})
                <p>
                  Categoria: {product.category} | Preço: R${" "}
                  {formatPriceDisplay(product.price)} | Estoque: {product.stock}
                </p>
              </div>
              <div className="item-actions">
                <button type="button" onClick={() => handleEdit(product)}>
                  Editar
                </button>
                <button
                  type="button"
                  className="danger"
                  onClick={() => handleDelete(product.id)}
                >
                  Remover
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
