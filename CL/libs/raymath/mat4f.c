#include <raymath.h>


mat4 rm_mat4_create(float4 column0, float4 column1, float4 column2, float4 column3)
{
    float d[16] = {
        column0.x, column0.y, column0.z, column0.w,
        column1.x, column1.y, column1.z, column1.w,
        column2.x, column2.y, column2.z, column2.w,
        column3.x, column3.y, column3.z, column3.w
    };
    return (mat4){
        d
    };
}

float4 rm_mat4_mult_float4(mat4 lhs, float4 vector)
{
    float4 res;
    res.x = lhs.data[0] * vector.x + lhs.data[1] * vector.y + lhs.data[2] * vector.z + lhs.data[3] * vector.w;
    res.y = lhs.data[4] * vector.x + lhs.data[5] * vector.y + lhs.data[6] * vector.z + lhs.data[7] * vector.w;
    res.z = lhs.data[8] * vector.x + lhs.data[9] * vector.y + lhs.data[10] * vector.z + lhs.data[11] * vector.w;
    res.w = lhs.data[12] * vector.x + lhs.data[13] * vector.y + lhs.data[14] * vector.z + lhs.data[15] * vector.w;
    return res;
}

mat4 rm_mat4_mult_mat4(mat4 lhs, mat4 rhs)
{
    mat4 res = rm_mat4_create((float4)(0.0F),(float4)(0.0F),(float4)(0.0F),(float4)(0.0F));
    
    /*for(int a = 0; a < 4; a++)
        for(int b = 0; b < 4; b++)
            res.data[b+a*4] = lhs.data[4+a] * rhs.data[b] + lhs.data[a*4+1] * rhs.data[b+4] + lhs.data[a*4+2] * rhs.data[b+8] + lhs.data[a*4+3] * rhs.data[b+12];
*/
    res.data[0] = lhs.data[0] * rhs.data[0] + lhs.data[1] * rhs.data[4] + lhs.data[2] * rhs.data[8] + lhs.data[3] * rhs.data[12];
    res.data[1] = lhs.data[0] * rhs.data[1] + lhs.data[1] * rhs.data[5] + lhs.data[2] * rhs.data[9] + lhs.data[3] * rhs.data[13];
    res.data[2] = lhs.data[0] * rhs.data[2] + lhs.data[1] * rhs.data[6] + lhs.data[2] * rhs.data[10] + lhs.data[3] * rhs.data[14];
    res.data[3] = lhs.data[0] * rhs.data[3] + lhs.data[1] * rhs.data[7] + lhs.data[2] * rhs.data[11] + lhs.data[3] * rhs.data[15];

    res.data[4] = lhs.data[4] * rhs.data[0] + lhs.data[5] * rhs.data[4] + lhs.data[6] * rhs.data[8] + lhs.data[7] * rhs.data[12];
    res.data[5] = lhs.data[4] * rhs.data[1] + lhs.data[5] * rhs.data[5] + lhs.data[6] * rhs.data[9] + lhs.data[7] * rhs.data[13];
    res.data[6] = lhs.data[4] * rhs.data[2] + lhs.data[5] * rhs.data[6] + lhs.data[6] * rhs.data[10] + lhs.data[7] * rhs.data[14];
    res.data[7] = lhs.data[4] * rhs.data[3] + lhs.data[5] * rhs.data[7] + lhs.data[6] * rhs.data[11] + lhs.data[7] * rhs.data[15];

    res.data[8] = lhs.data[8] * rhs.data[0] + lhs.data[9] * rhs.data[4] + lhs.data[10] * rhs.data[8] + lhs.data[11] * rhs.data[12];
    res.data[9] = lhs.data[8] * rhs.data[1] + lhs.data[9] * rhs.data[5] + lhs.data[10] * rhs.data[9] + lhs.data[11] * rhs.data[13];
    res.data[10] = lhs.data[8] * rhs.data[2] + lhs.data[9] * rhs.data[6] + lhs.data[10] * rhs.data[10] + lhs.data[11] * rhs.data[14];
    res.data[11] = lhs.data[8] * rhs.data[3] + lhs.data[9] * rhs.data[7] + lhs.data[10] * rhs.data[11] + lhs.data[11] * rhs.data[15];

    res.data[12] = lhs.data[12] * rhs.data[0] + lhs.data[13] * rhs.data[4] + lhs.data[14] * rhs.data[8] + lhs.data[15] * rhs.data[12];
    res.data[13] = lhs.data[12] * rhs.data[1] + lhs.data[13] * rhs.data[5] + lhs.data[14] * rhs.data[9] + lhs.data[15] * rhs.data[13];
    res.data[14] = lhs.data[12] * rhs.data[2] + lhs.data[13] * rhs.data[6] + lhs.data[14] * rhs.data[10] + lhs.data[15] * rhs.data[14];
    res.data[15] = lhs.data[12] * rhs.data[3] + lhs.data[13] * rhs.data[7] + lhs.data[14] * rhs.data[11] + lhs.data[15] * rhs.data[15];

    return res;
}

mat4 rm_mat4_axisAngle(float3 axis, float angle)
{
    angle *= 0.017453F;

    float x = axis.x, y = axis.y, z = axis.z;
    float sa = sin(angle), ca = cos(angle);
    float xx = x * x, yy = y * y, zz = z * z;
    float xy = x * y, xz = x * z, yz = y * z;
 
    mat4 result = rm_mat4_create((float4)(0.0F),(float4)(0.0F),(float4)(0.0F),(float4)(0.0F));
 
    result.data[0] = xx + ca * (1.0F - xx);
    result.data[1] = xy - ca * xy + sa * z;
    result.data[2] = xz - ca * xz - sa * y;
    result.data[3] = 0.0F;

    result.data[4] = xy - ca * xy - sa * z;
    result.data[5] = yy + ca * (1.0F - yy);
    result.data[6] = yz - ca * yz + sa * x;
    result.data[7] = 0.0F;

    result.data[8] = xz - ca * xz + sa * y;
    result.data[9] = yz - ca * yz - sa * x;
    result.data[10] = zz + ca * (1.0F - zz);
    result.data[11] = 0.0F;

    result.data[12] = 0.0F;
    result.data[13] = 0.0F;
    result.data[14] = 0.0F;
    result.data[15] = 1.0F;
 
    return result;
}

mat4 rm_mat4_scale(float3 scale, float3 center)
{
    float tx = center.x * (1.0F - scale.x);
    float ty = center.y * (1.0F - scale.y);
    float tz = center.z * (1.0F - scale.z);

    return rm_mat4_create(
        (float4)(scale.x, 0.0F, 0.0F, 0.0F),
        (float4)(0.0F, scale.y, 0.0F, 0.0F),
        (float4)(0.0F, 0.0F, scale.z, 0.0F),
        (float4)( tx  ,  ty  ,  tz  , 1.0F)
        );
}

mat4 rm_mat4_translate(float3 v)
{
    return rm_mat4_create(
        //(float4)(1.0F, 0.0F, 0.0F, v.x),
        //(float4)(0.0F, 1.0F, 0.0F, v.y),
        //(float4)(0.0F, 0.0F, 1.0F, v.z),
        //(float4)(0.0F, 0.0F, 0.0F, 1.0F));
        (float4) { 1.0F, 0.0F, 0.0F, 0.0F},
        (float4) { 0.0F, 1.0F, 0.0F, 0.0F},
        (float4) { 0.0F, 0.0F, 1.0F, 0.0F},
		(float4) { v.x,   v.y,   v.z,   1.0F });
}

mat4 rm_mat4_transpose(mat4 * src, mat4 * dst)
{
    for (int i = 0; i < 16; i++)
    {
        int row = i / 4;
        int col = i % 4;

        dst->data[i] = src->data[4 * col + row];
    }
}

mat4 rm_mat4_rotate(quaternion q)
{
    // Precalculate coordinate products
    float x = q.x * 2.0F;
    float y = q.y * 2.0F;
    float z = q.z * 2.0F;
    float xx = q.x * x;
    float yy = q.y * y;
    float zz = q.z * z;
    float xy = q.x * y;
    float xz = q.x * z;
    float yz = q.y * z;
    float wx = q.w * x;
    float wy = q.w * y;
    float wz = q.w * z;

    mat4 m = rm_mat4_create((float4)(0.0F), (float4)(0.0F), (float4)(0.0F), (float4)(0.0F));

    m.data[0] = 1.0f - (yy + zz); m.data[4] = xy + wz; m.data[8] = xz - wy; m.data[12] = 0.0F;
    m.data[1] = xy - wz; m.data[5] = 1.0f - (xx + zz); m.data[9] = yz + wx; m.data[13] = 0.0F;
    m.data[2] = xz + wy; m.data[6] = yz - wx; m.data[10] = 1.0f - (xx + yy); m.data[14] = 0.0F;
    m.data[3] = 0.0F; m.data[7] = 0.0F; m.data[11] = 0.0F; m.data[14] = 1.0F;

    return m;

}

float rm_mat4_determinant(mat4 mat)
{
    float a = mat.data[0], b = mat.data[1], c = mat.data[2], d = mat.data[3];
    float e = mat.data[4], f = mat.data[5], g = mat.data[6], h = mat.data[7];
    float i = mat.data[8], j = mat.data[9], k = mat.data[10], l = mat.data[11];
    float m = mat.data[12], n = mat.data[13], o = mat.data[14], p = mat.data[15];
 
    float kp_lo = k * p - l * o;
    float jp_ln = j * p - l * n;
    float jo_kn = j * o - k * n;
    float ip_lm = i * p - l * m;
    float io_km = i * o - k * m;
    float in_jm = i * n - j * m;
 
    return 
        a * (f * kp_lo - g * jp_ln + h * jo_kn) -
        b * (e * kp_lo - g * ip_lm + h * io_km) +
        c * (e * jp_ln - f * ip_lm + h * in_jm) -
        d * (e * jo_kn - f * io_km + g * in_jm);
}

mat4 rm_mat4_invert(mat4 matrix)
{
    /*mat4 inverse = rm_mat4_create((float4)(0.0F), (float4)(0.0F), (float4)(0.0F), (float4)(0.0F));
    rm_mat4_transpose(&mat, &inverse);
    
    inverse.data[12] = inverse.data[3] = 0.0f;
	inverse.data[13] = inverse.data[7] = 0.0f;
	inverse.data[14] = inverse.data[11] = 0.0f;

    float3 right =      (float3)(mat.data[0], mat.data[1], mat.data[2]);
	float3 up =         (float3)(mat.data[4], mat.data[5], mat.data[6]);
	float3 forward =    (float3)(mat.data[8], mat.data[9], mat.data[10]);
	float3 position =   (float3)(mat.data[12], mat.data[13], mat.data[14]);

    inverse.data[12] = -dot(right, position);
	inverse.data[13] = -dot(up, position);
	inverse.data[14] = -dot(forward, position);

    return inverse;*/

    mat4 result = rm_mat4_create((float4)(0.0F), (float4)(0.0F), (float4)(0.0F), (float4)(0.0F));

    float a = matrix.data[0], b = matrix.data[1], c = matrix.data[2], d = matrix.data[3];
    float e = matrix.data[4], f = matrix.data[5], g = matrix.data[6], h = matrix.data[7];
    float i = matrix.data[8], j = matrix.data[9], k = matrix.data[10], l = matrix.data[11];
    float m = matrix.data[12], n = matrix.data[13], o = matrix.data[14], p = matrix.data[15];
    
    float kp_lo = k * p - l * o;
    float jp_ln = j * p - l * n;
    float jo_kn = j * o - k * n;
    float ip_lm = i * p - l * m;
    float io_km = i * o - k * m;
    float in_jm = i * n - j * m;

    float a11 = +(f * kp_lo - g * jp_ln + h * jo_kn);
    float a12 = -(e * kp_lo - g * ip_lm + h * io_km);
    float a13 = +(e * jp_ln - f * ip_lm + h * in_jm);
    float a14 = -(e * jo_kn - f * io_km + g * in_jm);
 
    float det = a * a11 + b * a12 + c * a13 + d * a14;

    if(fabs(det) < F_EPSILON)
    {
        return result;
        //return false;
    }

    float invDet = 1.0F / det;

    result.data[0] = a11 * invDet;
    result.data[4] = a12 * invDet;
    result.data[8] = a13 * invDet;
    result.data[12] = a14 * invDet;

    result.data[1] = -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet;
    result.data[5] = +(a * kp_lo - c * ip_lm + d * io_km) * invDet;
    result.data[9] = -(a * jp_ln - b * ip_lm + d * in_jm) * invDet;
    result.data[13] = +(a * jo_kn - b * io_km + c * in_jm) * invDet;

    float gp_ho = g * p - h * o;
    float fp_hn = f * p - h * n;
    float fo_gn = f * o - g * n;
    float ep_hm = e * p - h * m;
    float eo_gm = e * o - g * m;
    float en_fm = e * n - f * m;

    result.data[2] = +(b * gp_ho - c * fp_hn + d * fo_gn) * invDet;
    result.data[6] = -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet;
    result.data[10] = +(a * fp_hn - b * ep_hm + d * en_fm) * invDet;
    result.data[14] = -(a * fo_gn - b * eo_gm + c * en_fm) * invDet;

    float gl_hk = g * l - h * k;
    float fl_hj = f * l - h * j;
    float fk_gj = f * k - g * j;
    float el_hi = e * l - h * i;
    float ek_gi = e * k - g * i;
    float ej_fi = e * j - f * i;
 
    result.data[3] = -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet;
    result.data[7] = +(a * gl_hk - c * el_hi + d * ek_gi) * invDet;
    result.data[11] = -(a * fl_hj - b * el_hi + d * ej_fi) * invDet;
    result.data[15] = +(a * fk_gj - b * ek_gi + c * ej_fi) * invDet;
 
    return result;
}

/*mat4 rm_mat4_inverse(mat4 m)
{
    return rm_mat4_create(
        (float4)(m.data[0], m.data[1], m.data[2], 0.0F),

        (float4)(m.data[4], m.data[5], m.data[6], 0.0F),

        (float4)(m.data[8], m.data[9], m.data[10], 0.0F),

        (float4)(
            -dot((float3)(m.data[0], m.data[4], m.data[8]), (float3)(m.data[3], m.data[7], m.data[11])), 
            -dot((float3)(m.data[1], m.data[5], m.data[9]), (float3)(m.data[3], m.data[7], m.data[11])),
            -dot((float3)(m.data[2], m.data[6], m.data[10]), (float3)(m.data[3], m.data[7], m.data[11])),
            1.0F)
    );
}*/

mat4 rm_mat4_cofactor(mat4 m)
{
    mat4 ans = rm_mat4_create((float4)(0.0F),(float4)(0.0F),(float4)(0.0F),(float4)(0.0F));

    float det = (m.data[0] * m.data[6] * m.data[10] * m.data[15] ) + (m.data[0] * m.data[7] * m.data[11] * m.data[13] ) + (m.data[0] * m.data[8] * m.data[9] * m.data[14] ) 
    - (m.data[0] * m.data[8] * m.data[10] * m.data[13] ) - (m.data[0] * m.data[7] * m.data[9] * m.data[15] ) - (m.data[0] * m.data[6] * m.data[11] * m.data[14] ) 
    - (m.data[1] * m.data[5] * m.data[10] * m.data[15] ) - (m.data[2] * m.data[5] * m.data[11] * m.data[13] ) - (m.data[3] * m.data[5] * m.data[9] * m.data[14] )
    + (m.data[3] * m.data[5] * m.data[10] * m.data[13] ) + (m.data[2] * m.data[5] * m.data[9] * m.data[15] ) + (m.data[1] * m.data[5] * m.data[11] * m.data[14] )
    + (m.data[1] * m.data[7] * m.data[9] * m.data[15] ) + (m.data[2] * m.data[8] * m.data[9] * m.data[13] ) + (m.data[3] * m.data[6] * m.data[9] * m.data[14] )
    - (m.data[3] * m.data[7] * m.data[9] * m.data[13] ) - (m.data[2] * m.data[6] * m.data[9] * m.data[15] ) - (m.data[1] * m.data[8] * m.data[9] * m.data[14] )
    - (m.data[1] * m.data[7] * m.data[11] * m.data[12] ) - (m.data[2] * m.data[8] * m.data[9] * m.data[12] ) - (m.data[3] * m.data[6] * m.data[10] * m.data[12] )
    + (m.data[3] * m.data[7] * m.data[9] * m.data[12] ) + (m.data[2] * m.data[6] * m.data[11] * m.data[12] ) + (m.data[1] * m.data[8] * m.data[10] * m.data[12] );


    ans.data[0] = (m.data[6]*m.data[10]*m.data[15] + m.data[7]*m.data[11]*m.data[13] + m.data[8]*m.data[9]*m.data[14] - m.data[8]*m.data[10]*m.data[13] - m.data[7]*m.data[9]*m.data[15] - m.data[6]*m.data[11]*m.data[14])/det;
    ans.data[1] = (-m.data[1]*m.data[10]*m.data[15] - m.data[2]*m.data[11]*m.data[13] - m.data[3]*m.data[9]*m.data[14] + m.data[3]*m.data[10]*m.data[13] + m.data[2]*m.data[9]*m.data[15] + m.data[1]*m.data[11]*m.data[14])/det;
    ans.data[2] = (m.data[1]*m.data[7]*m.data[15] + m.data[2]*m.data[8]*m.data[13] + m.data[3]*m.data[6]*m.data[14] - m.data[3]*m.data[7]*m.data[13] - m.data[2]*m.data[6]*m.data[15] - m.data[1]*m.data[8]*m.data[14])/det;
    ans.data[3] = (-m.data[1]*m.data[7]*m.data[11] - m.data[2]*m.data[8]*m.data[9] - m.data[3]*m.data[6]*m.data[10] + m.data[3]*m.data[7]*m.data[9] + m.data[2]*m.data[6]*m.data[11] + m.data[1]*m.data[8]*m.data[10])/det;
    ans.data[4] = (-m.data[5]*m.data[10]*m.data[15] - m.data[7]*m.data[11]*m.data[12] - m.data[8]*m.data[9]*m.data[14] + m.data[8]*m.data[10]*m.data[12] + m.data[7]*m.data[9]*m.data[15] + m.data[5]*m.data[11]*m.data[14])/det;
    ans.data[5] = (m.data[0]*m.data[10]*m.data[15] + m.data[2]*m.data[11]*m.data[12] + m.data[3]*m.data[9]*m.data[14] - m.data[3]*m.data[10]*m.data[12] - m.data[2]*m.data[9]*m.data[15] - m.data[0]*m.data[11]*m.data[14])/det;
    ans.data[6] = (-m.data[0]*m.data[7]*m.data[15] - m.data[2]*m.data[8]*m.data[12] - m.data[3]*m.data[5]*m.data[14] + m.data[3]*m.data[7]*m.data[12] + m.data[2]*m.data[5]*m.data[15] + m.data[0]*m.data[8]*m.data[14])/det;
    ans.data[7] = (m.data[0]*m.data[7]*m.data[11] + m.data[2]*m.data[8]*m.data[9] + m.data[3]*m.data[5]*m.data[10] - m.data[3]*m.data[7]*m.data[9] - m.data[2]*m.data[5]*m.data[11] - m.data[0]*m.data[8]*m.data[10])/det;
    ans.data[8] = (m.data[5]*m.data[9]*m.data[15] + m.data[6]*m.data[11]*m.data[12] + m.data[8]*m.data[9]*m.data[13] - m.data[8]*m.data[9]*m.data[12] - m.data[6]*m.data[9]*m.data[15] - m.data[5]*m.data[11]*m.data[13])/det;
    ans.data[9] = (-m.data[0]*m.data[9]*m.data[15] - m.data[1]*m.data[11]*m.data[12] - m.data[3]*m.data[9]*m.data[13] + m.data[3]*m.data[9]*m.data[12] + m.data[1]*m.data[9]*m.data[15] + m.data[0]*m.data[11]*m.data[13])/det;
    ans.data[10] = (m.data[0]*m.data[6]*m.data[15] + m.data[1]*m.data[8]*m.data[12] + m.data[3]*m.data[5]*m.data[13] - m.data[3]*m.data[6]*m.data[12] - m.data[1]*m.data[5]*m.data[15] - m.data[0]*m.data[8]*m.data[13])/det;
    ans.data[11] = (-m.data[0]*m.data[6]*m.data[11] - m.data[1]*m.data[8]*m.data[9] - m.data[3]*m.data[5]*m.data[9] + m.data[3]*m.data[6]*m.data[9] + m.data[1]*m.data[5]*m.data[11] + m.data[0]*m.data[8]*m.data[9])/det;
    ans.data[12] = (-m.data[5]*m.data[9]*m.data[14] - m.data[6]*m.data[10]*m.data[12] - m.data[7]*m.data[9]*m.data[13] + m.data[7]*m.data[9]*m.data[12] + m.data[6]*m.data[9]*m.data[14] + m.data[5]*m.data[10]*m.data[13])/det;
    ans.data[13] = (m.data[0]*m.data[9]*m.data[14] + m.data[1]*m.data[10]*m.data[12] + m.data[2]*m.data[9]*m.data[13] - m.data[2]*m.data[9]*m.data[12] - m.data[1]*m.data[9]*m.data[14] - m.data[0]*m.data[10]*m.data[13])/det;
    ans.data[14] = (-m.data[0]*m.data[6]*m.data[14] - m.data[1]*m.data[7]*m.data[12] - m.data[2]*m.data[5]*m.data[13] + m.data[2]*m.data[6]*m.data[12] + m.data[1]*m.data[5]*m.data[14] + m.data[0]*m.data[7]*m.data[13])/det;
    ans.data[15] = (m.data[0]*m.data[6]*m.data[10] + m.data[1]*m.data[7]*m.data[9] + m.data[2]*m.data[5]*m.data[9] - m.data[2]*m.data[6]*m.data[9] - m.data[1]*m.data[5]*m.data[10] - m.data[0]*m.data[7]*m.data[9])/det;



    return ans;
}

