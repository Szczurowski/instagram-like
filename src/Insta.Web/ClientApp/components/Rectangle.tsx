import * as React from 'react';
import { FaceRectangle } from "../types/photoDetailed";

export interface RectangleStyle {
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

export interface RectangleComputeSize {
    conHeight: number;
    conWidth: number;
    imgWidth: number;
}

interface RectangeProperties {
    faceRectangle: FaceRectangle;
    sizes?: RectangleComputeSize;
}

export class Rectangle extends React.Component<RectangeProperties, {}> {
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
            top: this.computePercentage(top, sizes.conHeight),
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