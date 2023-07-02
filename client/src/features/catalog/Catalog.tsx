import { Product } from "../../app/models/product";
import ProductList from "./ProductList";
import { useState, useEffect } from "react";


// export default function Catalog(props: Props) 
// {products, addProduct} to add curly brackets and then we can specify properties we're interested in the structuring.
// And then what this means is that we no longer need to user props.products or props.addProduct, just use products and addProduct directly.
export default function Catalog() {

    const [products, setProducts] = useState<Product[]>([]);

    useEffect(() => {
        fetch("http://localhost:5000/api/Products").then(response => response.json()).then(data => setProducts(data));
    }, []);


    return (
        <>
            <ProductList products={products}/>
        </>
    );
}