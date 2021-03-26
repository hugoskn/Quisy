import React, { Component } from 'react';

export class FetchData extends Component {
    static displayName = FetchData.name;

    constructor(props) {
        super(props);
        this.state = { products: [], loading: null, loadingMessage: 'Write something to compare prices', query: 'tv 50 inch' };
    }

    componentDidMount() {
        this.populateProductsData();
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
                            <td><a href={product.link} target='_blank'>{product.title}</a></td>
                            <td>
                                <a href={product.link} target='_blank'>
                                    <img style={{ height: '50px' }} src={product.image} alt={product.Title} />
                                </a>
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
            ? <p><em>{this.state.loadingMessage}</em></p>
            : this.state.loading
                ? <p><em>Loading...</em></p>
                : FetchData.renderProductsTable(this.state.products);

        return (
            <div>
                <h1 id="tabelLabel" >Products search</h1>
                <p>Please enter a product to search</p>
                <div>
                    <input type='text' onChange={(e) => this.setState({ query: e.target.value })} />
                    <button onClick={() => this.populateProductsData()}>Search</button>
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
        if (!response || response.status < 200 || response.status >= 300) {
            this.setState({ loading: null, loadingMessage: 'Sorry no products found' });
        }
        const data = await response.json();
        if (!data) {
            this.setState({ loading: null, loadingMessage: 'Sorry no products found' });
        }

        this.setState({ products: data, loading: false });
    }
}
