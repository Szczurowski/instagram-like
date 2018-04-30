import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { Result } from "../types/common";
import { PhotoDetailed, ProcessingAnalysisResult, FaceRectangle } from "../types/photoDetailed";

interface PhotoDetailedState {
    content: PhotoDetailed;
    sizes?: RectangleComputeSize;
    isLoading: boolean;
}

interface PhotoDetailedParams {
    id: number;
}

interface RectangleStyle{
    top: any,
    left: any,
    width: any,
    height: any,
    position: any,
    borderColor: any,
    borderWidth: any,
    borderStyle: any,
    zIndex: any,
};

interface RectangleComputeSize {
    conHeight: number;
    conWidth: number;
    imgWidth: number;
}

interface RectangeProperties {
    faceRectangle: FaceRectangle;
    sizes?: RectangleComputeSize;
}

class Rectangle extends React.Component<RectangeProperties, {}> {
    render() {
        return <div style={this.getStyle()}></div>;
    }

    computePercentage(rect: number, container: number, additional?: number): string {
        let value = rect / container * 100;
        if (additional != undefined) {
            value += additional;
        }
        return `${Math.floor(value)}%`;
    }

    getStyle(): RectangleStyle | undefined {
        const { sizes } = this.props;
        if (sizes == undefined) {
            return undefined;
        }

        const { top, left, width, height } = this.props.faceRectangle;
        const leftAdditional = (sizes.conWidth - sizes.imgWidth) / 2 / sizes.conWidth * 100;
        return {
            top: this.computePercentage(top, sizes.conHeight ),
            left: this.computePercentage(left, sizes.conWidth, leftAdditional),
            width: this.computePercentage(width, sizes.conWidth),
            height: this.computePercentage(height, sizes.conHeight),
            position: "absolute",
            borderColor: "#0078d7",
            borderWidth: "3px",
            borderStyle: "solid",
            zIndex: 99
        };
    }
}

export class Details extends React.Component<RouteComponentProps<PhotoDetailedParams>, PhotoDetailedState> {
    constructor(props: RouteComponentProps<PhotoDetailedParams>) {
        super(props);

        this.imageLoaded = this.imageLoaded.bind(this);
        this.referenceImage = this.referenceImage.bind(this);
        this.referenceCon = this.referenceCon.bind(this);
        this.imageDeleting = this.imageDeleting.bind(this);

        this.state = {
            isLoading: true,
            content: {
                id: 0,
                name: '',
                originalLocation: '',
                processingAnalysisResult: {
                    description: {
                        captions: [],
                        tags: []
                    },
                    faces: []
                }
            }
        };
    }

    componentDidMount() {
        const { match: { params } } = this.props;

        fetch(`api/photo/${params.id}`)
            .then(response => response.json() as Promise<Result<PhotoDetailed>>)
            .then(({ content }) => {
                this.setState({ content, isLoading: false });
            });
    }

    render() {
        const { sizes } = this.state;
        const imgAvailable = sizes != undefined;
        const { content: { name, originalLocation, processingAnalysisResult } } = this.state;
        const captions = processingAnalysisResult.description.captions;
        const faceRectangles = this.getFaces(processingAnalysisResult);
        
        return <div>
            <h2>Details</h2>
            <h3>Computer Vision Analysis Results of file <b>{name}</b></h3>
            <button type="button" className="btn btn-danger" onClick={this.imageDeleting}>Delete</button>

            <div ref={this.referenceCon} style={{ position: "relative", textAlign: "center", margin: "0 auto" }}>
                <div>
                   <img ref={this.referenceImage} src={originalLocation} alt={name} style={{ maxWidth: "100%" }} onLoad={this.imageLoaded}/>
                        {imgAvailable && faceRectangles.map((faceRect, key) =>
                            <Rectangle key={key} faceRectangle={faceRect} sizes={sizes} />
                        )}
                </div>
            </div>
            <div style={{ marginTop: 20 }}>
                <b>Detected captions</b>
                <table className="table">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Value</th>
                            <th>Confidence</th>
                        </tr>
                    </thead>
                    <tbody>
                        {captions && captions.map((caption, i) =>
                        <tr>
                            <td>{`Caption ${i}`}</td>
                            <td>{caption.text}</td>
                            <td>{caption.confidence}</td>
                        </tr>
                        )}
                    </tbody>
                </table>
            </div>
       </div>;
    }

    getFaces(processingAnalysisResult: ProcessingAnalysisResult): FaceRectangle[] {
        const { faces } = processingAnalysisResult;
        if (faces.length < 1) {
            return [];
        }

        return faces.map(f => f.faceRectangle);
    }

    imageLoaded(e: any) {
        if (this.imageElement != null && this.conElement != null) {
            const imgWidth = this.imageElement.width;
            const conWidth = this.conElement.clientWidth;
            const conHeight = this.conElement.clientHeight;
            this.setState({ sizes: { imgWidth, conWidth, conHeight }});
        }
    }

    imageDeleting() {
        const { match: { params } } = this.props;
        fetch(`api/Photo/${params.id}`,
            {
                method: 'DELETE',
            })
            .then(response => {
                if (!response.ok) {
                    console.log(response.statusText);
                }
                return response.json() as Promise<{}>;
            })
            .then(_ => {
                this.props.history.push('/');
            })
            .catch(error => {
                this.setState({ isLoading: false });
                console.log(error);
            });
    }

    referenceImage(elem: HTMLImageElement | null) {
        this.imageElement = elem;
    }

    referenceCon(elem: HTMLDivElement | null) {
        this.conElement = elem;
    }

    imageElement: HTMLImageElement | null;

    conElement: HTMLDivElement | null;
}
