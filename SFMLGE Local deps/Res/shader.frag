precision highp float;

uniform sampler2D texture;
uniform vec2 resolution;

uniform float time;

uniform float waveScale;
uniform vec2 sinScale;
uniform vec2 timeScale;

void main()
{
    vec2 position = ( gl_TexCoord[0].xy * resolution.xy ); // convert from 0.0-1.0 to 0-1280 x 0-720

    position = position + vec2( sin(((time*timeScale.x) + (position.y * waveScale))) * sinScale.x, sin(((time*timeScale.y) + (position.x * waveScale))) * sinScale.y); // wavy!

    vec2 toScrnSpace = position.xy / resolution.xy; // convert from 0-1280 x 0-720 to 0.0-1.0

    // lookup the pixel in the texture
    vec4 pixel = texture2D(texture, toScrnSpace.xy);

    // multiply it by the color
    gl_FragColor = gl_Color * vec4(pixel.x,pixel.y,pixel.z,pixel.w);
}