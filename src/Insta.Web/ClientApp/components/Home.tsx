import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link } from "react-router-dom";
import 'isomorphic-fetch';
import { RingLoader } from 'react-spinners';
import { Result } from "../types/common";
import { Photo } from "../types/photo"

interface PhotoState {
    content: Photo[];
    isLoading: boolean;
}

export class Home extends React.Component<RouteComponentProps<{}>, PhotoState> {
    constructor(props: any) {
        super(props);
        this.handleChange = this.handleChange.bind(this);
        this.state = { content: [], isLoading: true }
    }

    componentDidMount() {
        this.fetchAllPhotos();
    }

    fetchAllPhotos() {
        this.setState({ isLoading: true });
        fetch('api/photo')
            .then(response => response.json() as Promise<Result<Photo[]>>)
            .then(({ content }) => {
                console.log(content);
                this.setState({ content, isLoading: false });
            });
    }

    render() {
        const { content } = this.state;

        return <div>
            <h1>Upload an image for analysis</h1>
                   <RingLoader
                       color={'#123abc'}
                       loading={this.state.isLoading} />
            
            <div style={{margin: 10}}>
                <input type="file" onChange={(e: React.ChangeEvent<HTMLInputElement>) => this.handleChange(e.target.files)} />
            </div>
            <div>
                {content.map(photo =>
                    <Link to={`/photoDetails/${photo.id}`}>
                        <img style={{ margin: 5 }} src={photo.thumbnailLocation} alt={photo.name} />
                    </Link>
                )}
            </div>
        </div>;
    }

    private handleChange(selectorFiles: FileList | null) {
        if (selectorFiles != null) {
            const file = selectorFiles[0];
            this.setState({ isLoading: true });
            fetch('api/Photo',
                    {
                        body: file,
                        method: 'POST',
                        headers: {
                            'x-filename': file.name
                        }
                    })
                .then(response => {
                    this.setState({ isLoading: false });
                    if (!response.ok) {
                        console.log(response.statusText);
                    }
                    return response.json() as Promise<{}>;
                })
                .then(_ => {
                    this.fetchAllPhotos();
                })
                .catch(error => {
                    this.setState({ isLoading: false });
                    console.log(error);
                });
        }
    }
}
