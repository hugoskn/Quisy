import React, { Component } from 'react';

export class FetchData extends Component {
    static displayName = FetchData.name;

    constructor(props) {
        super(props);
        this.state = { products: [], loading: null, query: null };
    }

    componentDidMount() {
        //this.populateProductsData('tv 50 inch');
    }

    static renderProductsTable(products) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Image</th>
                        <th>Title</th>
                        <th>Price</th>
                        <th>Source</th>
                    </tr>
                </thead>
                <tbody>
                    {products.map(product =>
                        <tr key={product.title}>
                            <td>{product.title}</td>
                            <td>
                                <img src={product.image} alt={product.Title} />
                            </td>
                            <td>${product.price}</td>
                            <td>{product.source}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading == null
            ? <p><em>Write something to compare prices</em></p>
            : this.state.loading
                ? <p><em>Loading...</em></p>
                : FetchData.renderProductsTable(this.state.products);

        return (
            <div>
                <h1 id="tabelLabel" >Products search</h1>
                <p>Please enter a product to search</p>
                <div>
                    <input type='text' onChange={(e) => this.setState({ query: e.target.value })} />
                    <button onClick={() => this.populateProductsData() }>Search</button>
                </div>
                {contents}
            </div>
        );
    }

    async populateProductsData() {
        if (!this.state.query || this.state.query.length <= 1) {
            alert("Please enter a product to search");
            return;
        }
        this.setState({ loading: true });
        const response = await fetch('api/products/All?query=' + this.state.query);
        const data = await response.json();
        this.setState({ products: data, loading: false });
    }
}
